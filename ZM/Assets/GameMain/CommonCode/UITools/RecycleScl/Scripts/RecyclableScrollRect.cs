//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFrameworkPackage
{
    /// <summary>
    /// Entry for the recycling system. Extends Unity's inbuilt ScrollRect.
    /// </summary>
    public class RecyclableScrollRect : ScrollRect
    {
        [HideInInspector]
        public IRecyclableScrollRectDataSource DataSource;

        public bool IsGrid;
        //Prototype cell can either be a prefab or present as a child to the content(will automatically be disabled in runtime)
        public RectTransform PrototypeCell;
        //If true the intiziation happens at Start. Controller must assign the datasource in Awake.
        //Set to false if self init is not required and use public init API.
        public bool SelfInitialize = false;

        public UnityEvent onDragEnd = new UnityEvent();

        public enum DirectionType
        {
            Vertical,
            Horizontal
        }

        public DirectionType Direction;

        //Segments : coloums for vertical and rows for horizontal.
        public int Segments
        {
            set
            {
                _segments = Math.Max(value, 2);
            }
            get
            {
                return _segments;
            }
        }
        
        [SerializeField]
        private int _segments;
        
        public RecyclingSystem recyclingSystem;
        private Vector2 _prevAnchoredPos;

        protected override void Start()
        {
            if (!Application.isPlaying) return;

            if (SelfInitialize) Initialize();
        }

        /// <summary>
        /// Initialization when selfInitalize is true. Assumes that data source is set in controller's Awake.
        /// </summary>
        private void Initialize(Action fnInitEnd = null)
        {
            //Contruct the recycling system.
            if (Direction == DirectionType.Vertical)
            {
                recyclingSystem = new VerticalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            else if (Direction == DirectionType.Horizontal)
            {
                recyclingSystem = new HorizontalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            vertical = Direction == DirectionType.Vertical;
            horizontal = Direction == DirectionType.Horizontal;

            _prevAnchoredPos = content.anchoredPosition;
            onValueChanged.RemoveListener(OnValueChangedListener);
            //Adding listener after pool creation to avoid any unwanted recycling behaviour.(rare scenerio)
            StartCoroutine(recyclingSystem.InitCoroutine(() =>
            { 
                onValueChanged.AddListener(OnValueChangedListener);
                if (fnInitEnd!=null)
                    fnInitEnd.Invoke();
            }));
        }

        /// <summary>
        /// public API for Initializing when datasource is not set in controller's Awake. Make sure selfInitalize is set to false. 
        /// </summary>
        public void Initialize(IRecyclableScrollRectDataSource dataSource,Action fnInitEnd = null)
        {
            DataSource = dataSource;
            
            if (recyclingSystem == null)
            {
                Initialize(fnInitEnd);
            }
            else
            {
                ReloadData();
            }
        }

        /// <summary>
        /// Added as a listener to the OnValueChanged event of Scroll rect.
        /// Recycling entry point for recyling systems.
        /// </summary>
        /// <param name="direction">scroll direction</param>
        public void OnValueChangedListener(Vector2 normalizedPos)
        {
            Vector2 dir = content.anchoredPosition - _prevAnchoredPos;
            m_ContentStartPosition += recyclingSystem.OnValueChangedListener(dir);
            _prevAnchoredPos = content.anchoredPosition;
        }

        /// <summary>
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData()
        {
            ReloadData(DataSource, _AddValueChangeListener);
        }

        /// <summary>
        /// Overloaded ReloadData with dataSource param
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData(IRecyclableScrollRectDataSource dataSource, Action a_fnOnSystemInitEnd)
        {
            if (recyclingSystem != null)
            {
                StopMovement();
                onValueChanged.RemoveListener(OnValueChangedListener);
                recyclingSystem.DataSource = dataSource;
                StartCoroutine(recyclingSystem.InitCoroutine(a_fnOnSystemInitEnd));
                _prevAnchoredPos = content.anchoredPosition;
            }
        }

        private void _AddValueChangeListener()
        {
            onValueChanged.AddListener(OnValueChangedListener);
        }

        public void RefreshScl()
        {
            if (recyclingSystem != null)
            {
                StopMovement();
                recyclingSystem.RefreshScl();
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            onDragEnd.Invoke();
        }

        /*
        #region Testing
        private void OnDrawGizmos()
        {
            if (_recyclableScrollRect is VerticalRecyclingSystem)
            {
                ((VerticalRecyclingSystem)_recyclableScrollRect).OnDrawGizmos();
            }

            if (_recyclableScrollRect is HorizontalRecyclingSystem)
            {
                ((HorizontalRecyclingSystem)_recyclableScrollRect).OnDrawGizmos();
            }

        }
        #endregion
        */
    }
}