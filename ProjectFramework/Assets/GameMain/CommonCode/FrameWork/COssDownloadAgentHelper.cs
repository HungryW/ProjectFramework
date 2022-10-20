using Aliyun.OSS;
using Aliyun.OSS.Common;
using GameFramework;
using GameFramework.Download;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public class COssMgr : ISingleton<COssMgr>
    {
        private const string accessKeyId = "HS1heUAde/uD7YoJ6ULui2ZF2BD2dN+wPugO7rp2fas=";
        private const string accessKeySecret = "xLiVAX7R0ulmlF8dRPqpuy2Q6rOE7I7icDwvxeANggQ=";
        private const string accessPassword = " b59J9tQyuY9ZRXWTtPBMkluiYE0AGh ";
        private const string endPoint = "https://oss-ap-southeast-1.aliyuncs.com";
        private string m_szBucketName = "";
        private int m_nObjectNameStartIdx = -1;

        private OssClient m_ossClient = null;

        public OssClient GetClient()
        {
            if (m_ossClient == null)
            {
                ClientConfiguration conf = new ClientConfiguration();
                // 设置请求失败后最大的重试次数。
                conf.MaxErrorRetry = 3;
                // 设置连接超时时间单位为毫秒。
                conf.ConnectionTimeout = 30 * 1000;
                //进度条更新间隔，单位为字节
                conf.ProgressUpdateInterval = 1024;
                m_ossClient = new OssClient(endPoint, CPackageUtility.Text.RijndaelDecrypt(accessKeyId, accessPassword), CPackageUtility.Text.RijndaelDecrypt(accessKeySecret, accessPassword), conf);
            }
            return m_ossClient;
        }

        public string GetBucketName(string downloadUri)
        {
            if (string.IsNullOrEmpty(m_szBucketName))
            {
                const string szBucketNamePreFix = "://";
                const char cBucketNameEndChar = '.';
                int nBucketNameStartIdx = downloadUri.IndexOf(szBucketNamePreFix) + szBucketNamePreFix.Length;
                int nBucketNameEndIdx = downloadUri.IndexOf(cBucketNameEndChar);
                int nBucketNameLen = nBucketNameEndIdx - nBucketNameStartIdx;
                // 填写Bucket名称，例如examplebucket。
                m_szBucketName = downloadUri.Substring(nBucketNameStartIdx, nBucketNameLen);
            }
            return m_szBucketName;
        }

        private int _GetObjectNameStartIdx(string downloadUri)
        {
            if (m_nObjectNameStartIdx == -1)
            {
                const string szObjectNamePreFix = ".com/";
                m_nObjectNameStartIdx = downloadUri.IndexOf(szObjectNamePreFix) + szObjectNamePreFix.Length;
            }
            return m_nObjectNameStartIdx;
        }

        public string GetObjectNameRoot(string downloadUri)
        {
            return downloadUri.Substring(_GetObjectNameStartIdx(downloadUri));
        }
    }


    /// <summary>
    /// 使用 阿里 oss  实现的下载代理辅助器。
    /// </summary>
    public class COssDownloadAgentHelper : DownloadAgentHelperBase
    {
        enum EState
        {
            Free = 0,
            Doing = 1,
            Compelte = 2,
        }
        private const int mc_nDownloadCacheLen = 0x1000;
        private byte[] m_downloadCache = new byte[mc_nDownloadCacheLen];

        private string m_szBucketName = "";
        private int m_nObjectNameStartIdx = -1;
        private EState m_state = EState.Free;
        private string m_szErrorMsg = null;
        private int m_nDownloadedTotalLen = 0;
        private int m_nDownloadedIncrementLen = 0;

        private EventHandler<DownloadAgentHelperUpdateLengthEventArgs> m_DownloadAgentHelperUpdateEventHandler = null;
        private EventHandler<DownloadAgentHelperUpdateBytesEventArgs> m_DownloadAgentHelperUpdateBytesEventHandler = null;
        private EventHandler<DownloadAgentHelperCompleteEventArgs> m_DownloadAgentHelperCompleteEventHandler = null;
        private EventHandler<DownloadAgentHelperErrorEventArgs> m_DownloadAgentHelperErrorEventHandler = null;

        /// <summary>
        /// 下载代理辅助器更新事件。
        /// </summary>
        public override event EventHandler<DownloadAgentHelperUpdateLengthEventArgs> DownloadAgentHelperUpdateLength
        {
            add
            {
                m_DownloadAgentHelperUpdateEventHandler += value;
            }
            remove
            {
                m_DownloadAgentHelperUpdateEventHandler -= value;
            }
        }

        public override event EventHandler<DownloadAgentHelperUpdateBytesEventArgs> DownloadAgentHelperUpdateBytes
        {
            add
            {
                m_DownloadAgentHelperUpdateBytesEventHandler += value;
            }
            remove
            {
                m_DownloadAgentHelperUpdateBytesEventHandler -= value;
            }
        }

        /// <summary>
        /// 下载代理辅助器完成事件。
        /// </summary>
        public override event EventHandler<DownloadAgentHelperCompleteEventArgs> DownloadAgentHelperComplete
        {
            add
            {
                m_DownloadAgentHelperCompleteEventHandler += value;
            }
            remove
            {
                m_DownloadAgentHelperCompleteEventHandler -= value;
            }
        }

        /// <summary>
        /// 下载代理辅助器错误事件。
        /// </summary>
        public override event EventHandler<DownloadAgentHelperErrorEventArgs> DownloadAgentHelperError
        {
            add
            {
                m_DownloadAgentHelperErrorEventHandler += value;
            }
            remove
            {
                m_DownloadAgentHelperErrorEventHandler -= value;
            }
        }

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据。
        /// </summary>
        /// <param name="downloadUri">下载地址。</param>
        /// <param name="userData">用户自定义数据。</param>
        public override void Download(string downloadUri, object userData)
        {
            _DownLoad(downloadUri, -1, -1, userData);
        }

        public override void Download(string downloadUri, long fromPosition, object userData)
        {
            _DownLoad(downloadUri, fromPosition, -1, userData);
            throw new NotImplementedException();
        }

        public override void Download(string downloadUri, long fromPosition, long toPosition, object userData)
        {
            _DownLoad(downloadUri, fromPosition, toPosition, userData);
        }

        private void _DownLoad(string downloadUri, long fromPosition, long toPosition, object userData)
        {
            if (m_DownloadAgentHelperUpdateEventHandler == null || m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("Download agent helper handler is invalid.");
                return;
            }

            if (m_state != EState.Free)
            {
                _OnDownloadFail(Utility.Text.Format("Download agent helper State Error m_state = {0}", m_state));
                return;
            }

            m_state = EState.Doing;
            string szBucketName = _GetBucketName(downloadUri);
            // 填写Object完整路径，完整路径中不能包含Bucket名称，例如exampledir/exampleobject.txt。
            string szObjectName = downloadUri.Substring(_GetObjectNameStartIdx(downloadUri));

            try
            {
                var getObjectRequest = new GetObjectRequest(szBucketName, szObjectName);
                if (fromPosition >= 0)
                {
                    getObjectRequest.SetRange(fromPosition, toPosition);
                }
                getObjectRequest.StreamTransferProgress += _StreamProgressCallback;
                string state = "Wait Download End";
                COssMgr.Instance.GetClient().BeginGetObject(getObjectRequest, _GetObjectCallback, state.Clone());

            }
            catch (OssException ex)
            {
                string szErrorMsg = Utility.Text.Format("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
                _OnDownloadFail(szErrorMsg);
            }
            catch (Exception ex)
            {
                _OnDownloadFail(ex.Message);
            }
        }
        private void _StreamProgressCallback(object sender, StreamTransferProgressArgs args)
        {
            m_nDownloadedIncrementLen = (int)args.IncrementTransferred;
            m_nDownloadedTotalLen = (int)args.TransferredBytes;
        }

        private void _GetObjectCallback(IAsyncResult ar)
        {
            try
            {
                if (m_state != EState.Doing)
                {
                    _OnDownloadFail("Download agent helper State Error");
                    return;
                }
                var ossObject = COssMgr.Instance.GetClient().EndGetObject(ar);
                using (var stream = ossObject.Content)
                {
                    _OnDownloadSuccess(stream);
                }
            }
            catch (Exception ex)
            {
                _OnDownloadFail(ex.Message);
            }
        }


        private void _OnDownloadSuccess(Stream result)
        {
            Array.Clear(m_downloadCache, 0, m_downloadCache.Length);
            int nBytesTotal = 0;
            int nBytesRead = 0;
            while ((nBytesRead = result.Read(m_downloadCache, nBytesTotal, m_downloadCache.Length)) > 0)
            {
                DownloadAgentHelperUpdateBytesEventArgs arg = DownloadAgentHelperUpdateBytesEventArgs.Create(m_downloadCache, 0, nBytesRead);
                m_DownloadAgentHelperUpdateBytesEventHandler(this, arg);
                ReferencePool.Release(arg);
                Array.Clear(m_downloadCache, 0, m_downloadCache.Length);
                nBytesTotal += nBytesRead;
            }

            m_state = EState.Compelte;
        }

        private void _OnDownloadFail(string a_szErrorMsg)
        {
            m_szErrorMsg = a_szErrorMsg;
            m_state = EState.Compelte;
        }


        private void Update()
        {
            if (m_state == EState.Doing)
            {
                if (m_nDownloadedIncrementLen > 0)
                {
                    m_nDownloadedIncrementLen = 0;
                    DownloadAgentHelperUpdateLengthEventArgs arg = DownloadAgentHelperUpdateLengthEventArgs.Create(m_nDownloadedTotalLen);
                    m_DownloadAgentHelperUpdateEventHandler(this, arg);
                    ReferencePool.Release(arg);
                }
                return;
            }

            if (m_state == EState.Compelte)
            {
                if (string.IsNullOrEmpty(m_szErrorMsg))
                {
                    DownloadAgentHelperCompleteEventArgs arg = DownloadAgentHelperCompleteEventArgs.Create(m_downloadCache.Length);
                    m_DownloadAgentHelperCompleteEventHandler(this, arg);
                    ReferencePool.Release(arg);
                }
                else
                {
                    DownloadAgentHelperErrorEventArgs arg = DownloadAgentHelperErrorEventArgs.Create(true, m_szErrorMsg);
                    m_DownloadAgentHelperErrorEventHandler(this, arg);
                    ReferencePool.Release(arg);
                }
            }
        }

        /// <summary>
        /// 重置下载代理辅助器。
        /// </summary>
        public override void Reset()
        {
            m_state = EState.Free;
            m_szErrorMsg = null;
            m_nDownloadedIncrementLen = 0;
            m_nDownloadedTotalLen = 0;
            Array.Clear(m_downloadCache, 0, m_downloadCache.Length);
        }

        public string _GetBucketName(string downloadUri)
        {
            if (string.IsNullOrEmpty(m_szBucketName))
            {
                const string szBucketNamePreFix = "://";
                const char cBucketNameEndChar = '.';
                int nBucketNameStartIdx = downloadUri.IndexOf(szBucketNamePreFix) + szBucketNamePreFix.Length;
                int nBucketNameEndIdx = downloadUri.IndexOf(cBucketNameEndChar);
                int nBucketNameLen = nBucketNameEndIdx - nBucketNameStartIdx;
                // 填写Bucket名称，例如examplebucket。
                m_szBucketName = downloadUri.Substring(nBucketNameStartIdx, nBucketNameLen);
            }
            return m_szBucketName;
        }

        private int _GetObjectNameStartIdx(string downloadUri)
        {
            if (m_nObjectNameStartIdx == -1)
            {
                const string szObjectNamePreFix = ".com/";
                m_nObjectNameStartIdx = downloadUri.IndexOf(szObjectNamePreFix) + szObjectNamePreFix.Length;
            }
            return m_nObjectNameStartIdx;
        }


    }
}
