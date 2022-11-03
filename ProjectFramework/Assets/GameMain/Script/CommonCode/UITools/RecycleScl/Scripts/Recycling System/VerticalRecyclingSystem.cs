//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{

    /// <summary>
    /// Recyling system for Vertical type.
    /// </summary>
    public class VerticalRecyclingSystem : RecyclingSystem
    {
        //Assigned by constructor
        private readonly int _coloumns;

        //Cell dimensions
        private float _cellWidth, _cellHeight;

        //Pool Generation
        private List<RectTransform> _cellPool;
        private List<ICell> _cachedCells;
        private Bounds _recyclableViewBounds;

        //Temps, Flags 
        private readonly Vector3[] _corners = new Vector3[4];
        private bool _recycling;

        //Trackers
        private int currentItemCount; //item count corresponding to the datasource.
        private int topMostCellIndex, bottomMostCellIndex; //Topmost and bottommost cell in the heirarchy
        private int _topMostCellColoumn, _bottomMostCellColoumn; // used for recyling in Grid layout. top-most and bottom-most coloumn

        //Cached zero vector 
        private Vector2 zeroVector = Vector2.zero;

        #region INIT
        public VerticalRecyclingSystem( RectTransform prototypeCell, RectTransform viewport, RectTransform content, IRecyclableScrollRectDataSource dataSource, bool isGrid, int coloumns)
        {
            PrototypeCell = prototypeCell;
            Viewport = viewport;
            Content = content;
            DataSource = dataSource;
            IsGrid = isGrid;
            _coloumns = isGrid ? coloumns : 1;
            _recyclableViewBounds = new Bounds();
        }

        /// <summary>
        /// Corotuine for initiazation.
        /// Using coroutine for init because few UI stuff requires a frame to update
        /// </summary>
        /// <param name="onInitialized">callback when init done</param>
        /// <returns></returns>>
        public override IEnumerator InitCoroutine(System.Action onInitialized)
        {
            SetTopAnchor(Content);
            Content.anchoredPosition = Vector3.zero;
            yield return null;
            SetRecyclingBounds();

            //Cell Poool
            CreateCellPool();
            int nActiveNum = _GetVisibleCellNum();
            currentItemCount = nActiveNum;
            topMostCellIndex = 0;
            bottomMostCellIndex = Mathf.Max(0, nActiveNum - 1);

            //Set content height according to no of rows
            _ResetContentSize();
            if (onInitialized != null) onInitialized();
        }

        private void _ResetContentSize()
        {
            int nActiveNum = _GetVisibleCellNum();
            int noOfRows = (int)Mathf.Ceil((float)nActiveNum / (float)_coloumns);
            float contentYSize = noOfRows * _cellHeight;
            Content.sizeDelta = new Vector2(Content.sizeDelta.x, contentYSize);
            SetTopAnchor(Content);
        }

        private int _GetVisibleCellNum()
        {
            int nNum = 0;
            for (int i = 0; i < _cellPool.Count; i++)
            {
                if (_cellPool[i].gameObject.activeSelf)
                {
                    nNum++;
                }
            }
            return nNum;
        }

        /// <summary>
        /// Sets the uppper and lower bounds for recycling cells.
        /// </summary>
        private void SetRecyclingBounds()
        {
            Viewport.GetWorldCorners(_corners);
            float threshHold = RecyclingThreshold * (_corners[2].y - _corners[0].y);
            _recyclableViewBounds.min = new Vector3(_corners[0].x, _corners[0].y - threshHold);
            _recyclableViewBounds.max = new Vector3(_corners[2].x, _corners[2].y + threshHold);
        }

        /// <summary>
        /// Creates cell Pool for recycling, Caches ICells
        /// </summary>
        private void CreateCellPool()
        {
            //Reseting Pool
            if (_cellPool != null)
            {
                _cellPool.ForEach((RectTransform item) => UnityEngine.Object.Destroy(item.gameObject));
                _cellPool.Clear();
                _cachedCells.Clear();
            }
            else
            {
                _cachedCells = new List<ICell>();
                _cellPool = new List<RectTransform>();
            }
            //Set the prototype cell active and set cell anchor as top 
            PrototypeCell.gameObject.SetActive(true);
            if (IsGrid)
            {
                SetTopLeftAnchor(PrototypeCell);
            }
            else
            {
                SetTopAnchor(PrototypeCell);
            }

            //Reset
            _topMostCellColoumn = _bottomMostCellColoumn = 0;

            //Temps
            float currentPoolCoverage = 0;
            int poolSize = 0;
            float posX = 0;
            float posY = 0;

            //set new cell size according to its aspect ratio
            _cellWidth = Content.rect.width / _coloumns;
            _cellHeight = PrototypeCell.sizeDelta.y / PrototypeCell.sizeDelta.x * _cellWidth;

            //Get the required pool coverage and mininum size for the Cell pool
            float requriedCoverage = MinPoolCoverage * Viewport.rect.height;
            int minPoolSize = Math.Min(MinPoolSize, DataSource.GetItemCount());

            //create cells untill the Pool area is covered and pool size is the minimum required
            while ((poolSize < minPoolSize || currentPoolCoverage < requriedCoverage))
            {
                //Instantiate and add to Pool
                RectTransform item = (UnityEngine.Object.Instantiate(PrototypeCell.gameObject)).GetComponent<RectTransform>();
                item.name = "Cell";
                item.sizeDelta = new Vector2(_cellWidth, _cellHeight);
                _cellPool.Add(item);
                item.SetParent(Content, false);

                if (IsGrid)
                {
                    posX = _bottomMostCellColoumn * _cellWidth;
                    item.anchoredPosition = new Vector2(posX, posY);
                    if (++_bottomMostCellColoumn >= _coloumns)
                    {
                        _bottomMostCellColoumn = 0;
                        posY -= _cellHeight;
                        currentPoolCoverage += item.rect.height;
                    }
                }
                else
                {
                    item.anchoredPosition = new Vector2(0, posY);
                    posY = item.anchoredPosition.y - item.rect.height;
                    currentPoolCoverage += item.rect.height;
                }

                //Setting data for Cell
                _cachedCells.Add(item.GetComponent<ICell>());

                if (poolSize < DataSource.GetItemCount())
                {
                    item.gameObject.SetActive(true);
                    DataSource.SetCell(_cachedCells[_cachedCells.Count - 1], poolSize);
                }
                else
                {
                    item.gameObject.SetActive(false);
                }


                //Update the Pool size
                poolSize++;
            }

            //TODO : you alrady have a _currentColoumn varaiable. Why this calculation?????
            if (IsGrid)
            {
                _bottomMostCellColoumn = (_bottomMostCellColoumn - 1 + _coloumns) % _coloumns;
            }

            //Deactivate prototype cell if it is not a prefab(i.e it's present in scene)
            if (PrototypeCell.gameObject.scene.IsValid())
            {
                PrototypeCell.gameObject.SetActive(false);
            }
        }
        #endregion

        #region RECYCLING
        /// <summary>
        /// Recyling entry point
        /// </summary>
        /// <param name="direction">scroll direction </param>
        /// <returns></returns>
        public override Vector2 OnValueChangedListener(Vector2 direction)
        {
            if (_recycling || _cellPool == null || _cellPool.Count == 0 || bottomMostCellIndex < 0 || topMostCellIndex < 0) return zeroVector;

            //Updating Recyclable view bounds since it can change with resolution changes.
            SetRecyclingBounds();
            int additionalRows = 0;
            if (direction.y > 0 && _cellPool[bottomMostCellIndex].MaxY() > _recyclableViewBounds.min.y)
            {
                return RecycleTopToBottom(out additionalRows);
            }
            else if (direction.y < 0 && _cellPool[topMostCellIndex].MinY() < _recyclableViewBounds.max.y)
            {
                return RecycleBottomToTop(out additionalRows);
            }

            return zeroVector;
        }

        /// <summary>
        /// Recycles cells from top to bottom in the List heirarchy
        /// </summary>
        private Vector2 RecycleTopToBottom(out int additionalRows)
        {
            _recycling = true;

            float posY = IsGrid ? _cellPool[bottomMostCellIndex].anchoredPosition.y : 0;
            float posX = 0;
            int n = 0;

            //to determine if content size needs to be updated
            additionalRows = 0;
            //Recycle until cell at Top is avaiable and current item count smaller than datasource
            while (_cellPool[topMostCellIndex].MinY() > _recyclableViewBounds.max.y && currentItemCount < DataSource.GetItemCount())
            {
                if (IsGrid)
                {
                    if (++_bottomMostCellColoumn >= _coloumns)
                    {
                        n++;
                        _bottomMostCellColoumn = 0;
                        posY = _cellPool[bottomMostCellIndex].anchoredPosition.y - _cellHeight;
                        additionalRows++;
                    }

                    //Move top cell to bottom
                    posX = _bottomMostCellColoumn * _cellWidth;
                    _cellPool[topMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (++_topMostCellColoumn >= _coloumns)
                    {
                        _topMostCellColoumn = 0;
                        additionalRows--;
                    }
                }
                else
                {
                    //Move top cell to bottom
                    posY = _cellPool[bottomMostCellIndex].anchoredPosition.y - _cellPool[bottomMostCellIndex].sizeDelta.y;
                    _cellPool[topMostCellIndex].anchoredPosition = new Vector2(_cellPool[topMostCellIndex].anchoredPosition.x, posY);
                }

                //Cell for row at
                _cellPool[topMostCellIndex].gameObject.SetActive(true);
                DataSource.SetCell(_cachedCells[topMostCellIndex], currentItemCount);

                //set new indices
                bottomMostCellIndex = topMostCellIndex;
                topMostCellIndex = (topMostCellIndex + 1) % _cellPool.Count;

                currentItemCount++;
                if (!IsGrid) n++;
            }

            //Content size adjustment 
            if (IsGrid)
            {
                Content.sizeDelta += additionalRows * Vector2.up * _cellHeight;
                //TODO : check if it is supposed to be done only when > 0
                if (additionalRows > 0)
                {
                    n -= additionalRows;
                }
            }

            //Content anchor position adjustment.
            _cellPool.ForEach((RectTransform cell) => cell.anchoredPosition += n * Vector2.up * _cellPool[topMostCellIndex].sizeDelta.y);
            Content.anchoredPosition -= n * Vector2.up * _cellPool[topMostCellIndex].sizeDelta.y;
            _recycling = false;
            Vector2 v2Offset = -new Vector2(0, n * _cellPool[topMostCellIndex].sizeDelta.y);
            return v2Offset;

        }

        /// <summary>
        /// Recycles cells from bottom to top in the List heirarchy
        /// </summary>
        private Vector2 RecycleBottomToTop(out int additionalRows)
        {
            _recycling = true;

            int n = 0;
            float posY = IsGrid ? _cellPool[topMostCellIndex].anchoredPosition.y : 0;
            float posX = 0;

            //to determine if content size needs to be updated
            additionalRows = 0;
            //Recycle until cell at bottom is avaiable and current item count is greater than cellpool size
            while (_cellPool[bottomMostCellIndex].MaxY() < _recyclableViewBounds.min.y && currentItemCount > _cellPool.Count)
            {

                if (IsGrid)
                {
                    if (--_topMostCellColoumn < 0)
                    {
                        n++;
                        _topMostCellColoumn = _coloumns - 1;
                        posY = _cellPool[topMostCellIndex].anchoredPosition.y + _cellHeight;
                        additionalRows++;
                    }

                    //Move bottom cell to top
                    posX = _topMostCellColoumn * _cellWidth;
                    _cellPool[bottomMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                    if (--_bottomMostCellColoumn < 0)
                    {
                        _bottomMostCellColoumn = _coloumns - 1;
                        additionalRows--;
                    }
                }
                else
                {
                    //Move bottom cell to top
                    posY = _cellPool[topMostCellIndex].anchoredPosition.y + _cellPool[topMostCellIndex].sizeDelta.y;
                    _cellPool[bottomMostCellIndex].anchoredPosition = new Vector2(_cellPool[bottomMostCellIndex].anchoredPosition.x, posY);
                    n++;
                }

                currentItemCount--;

                //Cell for row at
                _cellPool[topMostCellIndex].gameObject.SetActive(true);
                DataSource.SetCell(_cachedCells[bottomMostCellIndex], currentItemCount - _cellPool.Count);

                //set new indices
                topMostCellIndex = bottomMostCellIndex;
                bottomMostCellIndex = (bottomMostCellIndex - 1 + _cellPool.Count) % _cellPool.Count;
            }

            if (IsGrid)
            {
                Content.sizeDelta += additionalRows * Vector2.up * _cellHeight;
                //TODOL : check if it is supposed to be done only when > 0
                if (additionalRows > 0)
                {
                    n -= additionalRows;
                }
            }

            _cellPool.ForEach((RectTransform cell) => cell.anchoredPosition -= n * Vector2.up * _cellPool[topMostCellIndex].sizeDelta.y);
            Content.anchoredPosition += n * Vector2.up * _cellPool[topMostCellIndex].sizeDelta.y;
            _recycling = false;
            return new Vector2(0, n * _cellPool[topMostCellIndex].sizeDelta.y);
        }
        #endregion

        #region  HELPERS
        /// <summary>
        /// Anchoring cell and content rect transforms to top preset. Makes repositioning easy.
        /// </summary>
        /// <param name="rectTransform"></param>
        private void SetTopAnchor(RectTransform rectTransform)
        {
            //Saving to reapply after anchoring. Width and height changes if anchoring is change. 
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            //Setting top anchor 
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);

            //Reapply size
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        private void SetTopLeftAnchor(RectTransform rectTransform)
        {
            //Saving to reapply after anchoring. Width and height changes if anchoring is change. 
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            //Setting top anchor 
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);

            //Reapply size
            rectTransform.sizeDelta = new Vector2(width, height);
        }
        #endregion

        #region TESTING
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_recyclableViewBounds.min - new Vector3(2000, 0), _recyclableViewBounds.min + new Vector3(2000, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_recyclableViewBounds.max - new Vector3(2000, 0), _recyclableViewBounds.max + new Vector3(2000, 0));
        }
        #endregion

        public override void RefreshScl()
        {
            //开始的数据索引是 当前数据索引-显示的格子数
            //开始的游戏对象索引是 topMostCellIndex
            int nActiveNum = _GetVisibleCellNum();
            int nObjIdx = topMostCellIndex;
            int nDataIdx = currentItemCount - nActiveNum;
            int nObjNum = 0;
            int nTotalDataNum = DataSource.GetItemCount();

            //先刷新有数据的所有格子的显示
            while (nObjNum < _cellPool.Count)
            {
                if (nDataIdx < nTotalDataNum)
                {
                    DataSource.SetCell(_cachedCells[nObjIdx], nDataIdx);
                }
                nObjIdx = (nObjIdx + 1) % _cellPool.Count;
                nDataIdx++;
                nObjNum++;
            }


            //下面主要处理 添加或者删除格子的刷新
            //当数据没有填充满缓冲区时,刷新格子可见性
            if (nTotalDataNum < _cellPool.Count)
            {
                //重置遍历数据
                nObjIdx = topMostCellIndex;
                nDataIdx = currentItemCount - nActiveNum;
                nObjNum = 0;
                int nAfterActiveNum = 0;
                while (nObjNum < _cellPool.Count)
                {
                    if (nDataIdx < nTotalDataNum)
                    {
                        nAfterActiveNum++;
                        _cellPool[nObjIdx].gameObject.SetActive(true);
                    }
                    else
                    {
                        _cellPool[nObjIdx].gameObject.SetActive(false);
                    }
                    nObjIdx = (nObjIdx + 1) % _cellPool.Count;
                    nDataIdx++;
                    nObjNum++;
                }

                //增加或删除显示格子时 更新数据索引和最后一个游戏对象索引
                currentItemCount = currentItemCount + (nAfterActiveNum - nActiveNum);
                bottomMostCellIndex = (topMostCellIndex + nAfterActiveNum - 1) % _cellPool.Count;
                _ResetContentSize();
            }

            //当数据已经填充满所有缓冲格子
            else
            {
                int additionalRows = 0;
                //当前的数据索引大于数据总数只可能是删除格子了
                if (currentItemCount > nTotalDataNum)
                {
                    float posY = IsGrid ? _cellPool[topMostCellIndex].anchoredPosition.y : 0;
                    float posX = 0;

                    additionalRows = 0;
                    while (currentItemCount > nTotalDataNum)
                    {

                        if (IsGrid)
                        {
                            if (--_topMostCellColoumn < 0)
                            {
                                _topMostCellColoumn = _coloumns - 1;
                                posY = _cellPool[topMostCellIndex].anchoredPosition.y + _cellHeight;
                                additionalRows++;
                            }

                            //Move bottom cell to top
                            posX = _topMostCellColoumn * _cellWidth;
                            _cellPool[bottomMostCellIndex].anchoredPosition = new Vector2(posX, posY);

                            if (--_bottomMostCellColoumn < 0)
                            {
                                _bottomMostCellColoumn = _coloumns - 1;
                                additionalRows--;
                            }
                        }
                        else
                        {
                            //Move bottom cell to top
                            posY = _cellPool[topMostCellIndex].anchoredPosition.y + _cellPool[topMostCellIndex].sizeDelta.y;
                            _cellPool[bottomMostCellIndex].anchoredPosition = new Vector2(_cellPool[bottomMostCellIndex].anchoredPosition.x, posY);
                        }

                        currentItemCount--;

                        //回收的格子刷新显示
                        _cellPool[bottomMostCellIndex].gameObject.SetActive(true);
                        DataSource.SetCell(_cachedCells[bottomMostCellIndex], currentItemCount - _cellPool.Count);

                        //set new indices
                        topMostCellIndex = bottomMostCellIndex;
                        bottomMostCellIndex = (bottomMostCellIndex - 1 + _cellPool.Count) % _cellPool.Count;
                    }

                    if (IsGrid)
                    {
                        Content.sizeDelta += additionalRows * Vector2.up * _cellHeight;
                    }

                    //如果加行了，加的是最上面的行，content要向上提一个格子高度， 所有格子要向下降一个格子高度
                    if (additionalRows > 0)
                    {
                        Content.anchoredPosition += additionalRows * Vector2.up * _cellPool[topMostCellIndex].sizeDelta.y;
                        _cellPool.ForEach((RectTransform cell) => cell.anchoredPosition -= additionalRows * Vector2.up * _cellPool[topMostCellIndex].sizeDelta.y);
                    }
                }

                //当前的数据索引小于数据总数，有两种情况：
                //1.刷新之前currentItemCount == nTotalDataNum,刷新后currentItemCount < nTotalDataNum，这样就是加格子了
                //2.刷新之前currentItemCount < nTotalDataNum，那可能 加 减 不变三种情况，这种情况实际上只要刷新所有当前格子的显示就行了，但是种情况不好区分，要缓存之前的数据总数，
                //所以统一按照1的方式处理，将最上面的不显示的格子移到下面
                else if (currentItemCount < nTotalDataNum)
                {
                    if (nTotalDataNum == _cellPool.Count)
                    {
                        currentItemCount = _cellPool.Count;
                        bottomMostCellIndex = (topMostCellIndex + _cellPool.Count - 1) % _cellPool.Count;
                        _cellPool.ForEach((RectTransform cell) => cell.gameObject.SetActive(true));
                    }
                    else if (nTotalDataNum > _cellPool.Count)
                    {
                        RecycleTopToBottom(out additionalRows);
                        //如果减行了，减得是最上面的行， 所有格子要向上一个格子高度
                        if (IsGrid && additionalRows < 0)
                        {
                            _cellPool.ForEach((RectTransform cell) => cell.anchoredPosition -= additionalRows * Vector2.up * _cellPool[topMostCellIndex].sizeDelta.y);
                        }
                    }
                }
            }
        }
    }
}
