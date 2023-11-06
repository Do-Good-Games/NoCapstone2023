using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//
// Summary:
//     Abstract base class for the VerticalProgressBar.
public abstract class AbstractVerticalProgressBar : BindableElement, INotifyValueChanged<float>
{
    public new class UxmlTraits : BindableElement.UxmlTraits
    {
        private UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
        {
            name = "low-value",
            defaultValue = 0f
        };

        private UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
        {
            name = "high-value",
            defaultValue = 100f
        };

        private UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription
        {
            name = "title"
        };

        private UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
        {
            name = "value",
            defaultValue = 0f
        };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            AbstractVerticalProgressBar abstractProgressBar = ve as AbstractVerticalProgressBar;
            abstractProgressBar.lowValue = m_LowValue.GetValueFromBag(bag, cc);
            abstractProgressBar.highValue = m_HighValue.GetValueFromBag(bag, cc);
            abstractProgressBar.value = m_Value.GetValueFromBag(bag, cc);
            string valueFromBag = m_Title.GetValueFromBag(bag, cc);
            if (!string.IsNullOrEmpty(valueFromBag))
            {
                abstractProgressBar.title = valueFromBag;
            }
        }
    }

    //
    // Summary:
    //     USS Class Name used to style the VerticalProgressBar.
    public static readonly string ussClassName = "unity-vertical-progress-bar";

    //
    // Summary:
    //     USS Class Name used to style the container of the VerticalProgressBar.
    public static readonly string containerUssClassName = ussClassName + "__container";

    //
    // Summary:
    //     USS Class Name used to style the title of the VerticalProgressBar.
    public static readonly string titleUssClassName = ussClassName + "__title";

    //
    // Summary:
    //     USS Class Name used to style the container of the title of the VerticalProgressBar.
    public static readonly string titleContainerUssClassName = ussClassName + "__title-container";

    //
    // Summary:
    //     USS Class Name used to style the progress bar of the VerticalProgressBar.
    public static readonly string progressUssClassName = ussClassName + "__progress";

    //
    // Summary:
    //     USS Class Name used to style the background of the VerticalProgressBar.
    public static readonly string backgroundUssClassName = ussClassName + "__background";

    private readonly VisualElement m_Background;

    private readonly VisualElement m_Progress;

    private readonly Label m_Title;

    private float m_LowValue;

    private float m_HighValue = 100f;

    private float m_Value;

    private const float k_MinVisibleProgress = 1f;

    //
    // Summary:
    //     Sets the title of the ProgressBar that displays in the center of the control.
    public string title
    {
        get
        {
            return m_Title.text;
        }
        set
        {
            m_Title.text = value;
        }
    }

    //
    // Summary:
    //     Sets the minimum value of the ProgressBar.
    public float lowValue
    {
        get
        {
            return m_LowValue;
        }
        set
        {
            m_LowValue = value;
            SetProgress(m_Value);
        }
    }

    //
    // Summary:
    //     Sets the maximum value of the ProgressBar.
    public float highValue
    {
        get
        {
            return m_HighValue;
        }
        set
        {
            m_HighValue = value;
            SetProgress(m_Value);
        }
    }

    //
    // Summary:
    //     Sets the progress value. If the value has changed, dispatches an ChangeEvent_1
    //     of type float.
    public virtual float value
    {
        get
        {
            return m_Value;
        }
        set
        {
            if (EqualityComparer<float>.Default.Equals(m_Value, value))
            {
                return;
            }

            if (base.panel != null)
            {
                using ChangeEvent<float> changeEvent = ChangeEvent<float>.GetPooled(m_Value, value);
                changeEvent.target = this;
                SetValueWithoutNotify(value);
                SendEvent(changeEvent);
            }
            else
            {
                SetValueWithoutNotify(value);
            }
        }
    }

    public AbstractVerticalProgressBar()
    {
        AddToClassList(ussClassName);
        VisualElement visualElement = new VisualElement
        {
            name = ussClassName
        };
        m_Background = new VisualElement();
        m_Background.AddToClassList(backgroundUssClassName);
        visualElement.Add(m_Background);
        m_Progress = new VisualElement();
        m_Progress.AddToClassList(progressUssClassName);
        m_Background.Add(m_Progress);
        VisualElement visualElement2 = new VisualElement();
        visualElement2.AddToClassList(titleContainerUssClassName);
        m_Background.Add(visualElement2);
        m_Title = new Label();
        m_Title.AddToClassList(titleUssClassName);
        visualElement2.Add(m_Title);
        visualElement.AddToClassList(containerUssClassName);
        base.hierarchy.Add(visualElement);
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent e)
    {
        SetProgress(value);
    }

    //
    // Summary:
    //     Sets the progress value.
    //
    // Parameters:
    //   newValue:
    public void SetValueWithoutNotify(float newValue)
    {
        m_Value = newValue;
        SetProgress(value);
    }

    private void SetProgress(float p)
    {
        float height = ((p < lowValue) ? lowValue : ((!(p > highValue)) ? p : highValue));
        height = CalculateProgressHeight(height);
        if (height >= 0f)
        {
            m_Progress.style.top = height;
        }
    }

    private float CalculateProgressHeight(float height)
    {
        if (m_Background == null || m_Progress == null)
        {
            return 0f;
        }

        if (float.IsNaN(m_Background.layout.height))
        {
            return 0f;
        }

        float num = m_Background.layout.height - 2f;
        return num - Mathf.Max(num * height / highValue, 1f);
    }
}
