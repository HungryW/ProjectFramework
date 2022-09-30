using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class ScrollPageToolS : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public RectTransform itemContent;
    [SerializeField]
    private bool buttonPageEnable;
    private int m_nowPage;//从0开始
    private int m_pageCount;
    /// <summary>
    /// 滑动距离超过一页的 (m_dragNum*10)% 则滑动成功
    /// </summary>
    public int m_dragNum;
    public UnityEvent onDragEnd;
    private float m_pageAreaSize;
    private const float SCROLL_SMOOTH_TIME = 0.2F;
    private const float SCROLL_MOVE_SPEED = 1F;
    private float scrollMoveSpeed = 0f;
    private bool scrollNeedMove = false;
    private float scrollTargetValue;
    public ScrollRect scrollRect;

    private bool isRegistEvent = false;
    private bool isOffset = false;

    public bool SetButtonStatus
    {
        set
        {
            buttonPageEnable = value;
        }
    }

    void Awake()
    {
    }

    public void InitManager(int _allItemNum, Vector2 pageItemSize, bool isNeedChangeSize = true, int pageNum = 0, bool isShowAnim = false)
    {
        RegistEvent();
        int _pageItemNum = (int)(pageItemSize.x * pageItemSize.y);
        m_pageCount = (_allItemNum / _pageItemNum) + ((_allItemNum % _pageItemNum == 0) ? 0 : 1);
        m_pageAreaSize = 1f / (m_pageCount - 1);
        ChangePage(pageNum);
        
    }

    public void InitManager(int pageNum, int targetPage = 0, bool isShowAnim = false,bool bOffset = false)
    {
        isOffset = bOffset;
        RegistEvent();
        m_pageCount = pageNum;
        m_pageAreaSize = 1f / (m_pageCount - 1);
        ChangePage(targetPage, isShowAnim);
    }

    private void RegistEvent()
    {
        if (isRegistEvent)
            return;
        isRegistEvent = true;
    }

    private void Paging(int num)
    {
        //maxNum-1,从0开始
        num = (num < 0) ? -1 : 1;
        int temp = Mathf.Clamp(m_nowPage + num, 0, m_pageCount - 1);
        if (m_nowPage == temp)
            return;
        ChangePage(temp);
    }
    void Update()
    {
        ScrollControl();
    }

    public int GetPageNum { get { return m_nowPage; } }
    //按页翻动
    private void ScrollControl()
    {
        if (!scrollNeedMove)
            return;
        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - scrollTargetValue) < 0.01f)
        {
            scrollRect.horizontalNormalizedPosition = scrollTargetValue;
            scrollNeedMove = false;
            return;
        }
        scrollRect.horizontalNormalizedPosition = Mathf.SmoothDamp(scrollRect.horizontalNormalizedPosition, scrollTargetValue, ref scrollMoveSpeed, SCROLL_SMOOTH_TIME);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollNeedMove = false;
        scrollTargetValue = 0;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        int tempPage = m_nowPage;
        
        int num = (((scrollRect.horizontalNormalizedPosition - (m_nowPage * m_pageAreaSize)) >= 0) ? 1 : -1);

        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - (m_nowPage * m_pageAreaSize)) >= (m_pageAreaSize / 10f) * m_dragNum)
        {
            tempPage += num;

        }
        ChangePage(tempPage);
        onDragEnd.Invoke();
    }

    private void GetNum()
    {

    }

    public void ChangePage(int pageNum, bool isShowAnim = true)
    {
        if (pageNum >= m_pageCount)
            pageNum = m_pageCount - 1;
        if (pageNum < 0)
            pageNum = 0;

        m_nowPage = pageNum;
        ChangePageText(pageNum);
        if (isShowAnim)
        {
            scrollTargetValue = m_nowPage * m_pageAreaSize;
            scrollNeedMove = true;
            scrollMoveSpeed = 0;
        }
        else
        {
            scrollRect.horizontalNormalizedPosition = m_nowPage * m_pageAreaSize;
        }
        ChangePageText(m_nowPage);
    }

    public void ChangePageText(int num)
    {
        int maxPageTo0Start = m_pageCount - 1;
        m_nowPage = Mathf.Clamp(num, 0, maxPageTo0Start);
        
        //only one page
        if (maxPageTo0Start == 0)
        {
            scrollRect.enabled = false;
            return;
        }
        else
        {
            scrollRect.enabled = true;
        }
        SetButtonStatus = buttonPageEnable;
        if (!buttonPageEnable)
            return;
        


    }
}