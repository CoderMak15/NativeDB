using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.rac.ui
{
    public class UIContentCarousel : MonoBehaviour, IEndDragHandler, IBeginDragHandler
    {
        public System.Action<int> _onMovieSnap;

        public enum LayoutType
        {
            Vertical,
            Horizontal
        }

        [Header("Layout Settings")]
        public LayoutType _layoutType = LayoutType.Vertical;

        [Header("Content Viewport")]
        public float _pageSize = 250f;
        public float _swipeThreshold = 0.2f;
        public float _snapSpeed = 8f;

        [Header("Navigation Buttons")]
        public Button _nextButtons;
        public Button _prevButtons;

        [Header("Carousel Mode")]
        public bool _carouselMode = false;
        public bool _autoMove = false;
        public float _autoMoveTimer = 5f;

        [Header("Navigation Dots")]
        public GameObject _dotPrefab;
        public GameObject _dotsContainer;

        [Header("Dot Colors")]
        public Color _activeDotColor = Color.yellow; // Yellow/Goldish
        public Color _inactiveDotColor = Color.grey;
        public float dotColorTransitionSpeed = 5f;

        [Header("Dot Scaling")]
        public Vector2 _activeDotSize = new Vector2(20f, 10f);
        public Vector2 _inactiveDotSize = new Vector2(10f, 10f);
        public float _dotScalingSpeed = 5f;

        [Header("3D Rotation")]
        public float maxRotationAngle = 45f;
        public float rotationSpeed = 5f;

        [Header("(Experimental Features)")]
        [Header("Infinite Looping")]
        public bool infiniteLooping = true;

        [Header("Checking...")]
        public int totalPages;
        public int currentIndex = 0;
        public GridLayoutGroup gridLayoutGroup;

        private ScrollRect scrollRect;
        private RectTransform contentRectTransform;
        private float targetPosition;
        private bool isDragging = false;
        private Vector2 dragStartPos;
        private float lastDragTime;
        private float autoMoveTimerCountdown;
        private bool _init = false;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        }

        public void Init()
        {
            contentRectTransform = gridLayoutGroup.transform as RectTransform;
            gridLayoutGroup.startAxis = (_layoutType == LayoutType.Vertical) ? GridLayoutGroup.Axis.Vertical : GridLayoutGroup.Axis.Horizontal;

            CalculateTotalPages();
            SetSnapTarget(0);

            if (_carouselMode)
            {
                InitializeNavigationDots();
            }

            _nextButtons.onClick.AddListener(MoveToNextPage);
            _prevButtons.onClick.AddListener(MoveToPreviousPage);
            _init = true;
        }

        private void InitializeNavigationDots()
        {
            for (int i = 0; i < totalPages; i++)
            {
                GameObject dot = Instantiate(_dotPrefab, _dotsContainer.transform);
                SetDotSize(dot, i == currentIndex ? _activeDotSize : _inactiveDotSize);
                SetDotColor(dot, i == currentIndex ? _activeDotColor : _inactiveDotColor);
            }
        }

        private void SetDotColor(GameObject dot, Color color)
        {
            Image dotImage = dot.GetComponent<Image>();
            if (dotImage != null)
            {
                dotImage.color = color;
            }
        }

        private void SetDotSize(GameObject dot, Vector2 size)
        {
            RectTransform dotRect = dot.GetComponent<RectTransform>();
            if (dotRect != null)
            {
                dotRect.sizeDelta = size;
            }
        }

        private void CalculateTotalPages()
        {
            int itemCount = gridLayoutGroup.transform.childCount;
            totalPages = Mathf.CeilToInt((float)itemCount / gridLayoutGroup.constraintCount);
        }

        private void SetSnapTarget(int page)
        {
            targetPosition = -_pageSize * (page - totalPages / 2f);
            currentIndex = page;
            _onMovieSnap?.Invoke(currentIndex);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            dragStartPos = eventData.position;
            lastDragTime = Time.unscaledTime;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            float dragDistance = Mathf.Abs(eventData.position.x - dragStartPos.x);
            float dragSpeed = eventData.delta.x / (Time.unscaledTime - lastDragTime);

            if (_autoMove)
            {
                autoMoveTimerCountdown = _autoMoveTimer;
            }

            if (_carouselMode)
            {
                if (dragDistance > _pageSize * _swipeThreshold || Mathf.Abs(dragSpeed) > _swipeThreshold)
                {
                    int currentPage = Mathf.RoundToInt(contentRectTransform.anchoredPosition.x / -_pageSize);

                    if (dragSpeed > 0)
                    {
                        MoveToPreviousPage();
                    }
                    else
                    {
                        MoveToNextPage();
                    }
                }
                else
                {
                    SetSnapTarget(currentIndex);
                }
            }
        }

        private void Update()
        {
            if (!_init)
            {
                return;
            }

            if (_autoMove)
            {
                autoMoveTimerCountdown -= Time.deltaTime;
                if (autoMoveTimerCountdown <= 0f)
                {
                    MoveToNextPage();
                    autoMoveTimerCountdown = _autoMoveTimer;
                }
            }

            if (!isDragging)
            {
                contentRectTransform.anchoredPosition = Vector2.Lerp(
                    contentRectTransform.anchoredPosition,
                    new Vector2(targetPosition, contentRectTransform.anchoredPosition.y),
                    Time.deltaTime * _snapSpeed
                );

                UpdateDotSizes();
                RotateContent();
            }
        }

        private void RotateContent()
        {
            for (int i = 0; i < totalPages; i++)
            {
                GameObject content = gridLayoutGroup.transform.GetChild(i).gameObject;
                float rotationAngle = Mathf.Lerp(0f, maxRotationAngle, Mathf.Abs(currentIndex - i) / (float)totalPages);
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, rotationAngle);

                content.transform.rotation = Quaternion.Slerp(content.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }

        private void MoveToPreviousPage()
        {
            int prevPage = (currentIndex - 1 + totalPages) % totalPages;
            SetSnapTarget(prevPage);
        }

        private void MoveToNextPage()
        {
            int nextPage = (currentIndex + 1) % totalPages;
            SetSnapTarget(nextPage);
        }

        private void UpdateDotSizes()
        {
            for (int i = 0; i < _dotsContainer.transform.childCount; i++)
            {
                GameObject dot = _dotsContainer.transform.GetChild(i).gameObject;
                Vector2 targetSize = i == currentIndex ? _activeDotSize : _inactiveDotSize;
                RectTransform dotRect = dot.GetComponent<RectTransform>();

                if (dotRect != null)
                {
                    dotRect.sizeDelta = Vector2.Lerp(dotRect.sizeDelta, targetSize, Time.deltaTime * _dotScalingSpeed);
                }

                Image dotImage = dot.GetComponent<Image>();
                if (dotImage != null)
                {
                    Color targetColor = i == currentIndex ? _activeDotColor : _inactiveDotColor;
                    dotImage.color = Color.Lerp(dotImage.color, targetColor, Time.deltaTime * dotColorTransitionSpeed);
                }
            }
        }
    }
}