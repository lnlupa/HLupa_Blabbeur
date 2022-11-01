using Blabbeur.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Blabbeur
{
    namespace Unity
    {
        namespace UI
        {
            public class VariableUI : MonoBehaviour
            {
                private enum UIState
                { BOOL, INPUT };

                private UIState state = UIState.INPUT;
                private UIState State { get => state; set => SwitchToState(state); }

                public InputField nameInput;
                public InputField valueInput;
                public Toggle boolInput;
                public Text toggleText;
                public Dropdown typeDropdown;

                private ValueType Type { get => (ValueType)typeDropdown.value; }

                private string Name { get => nameInput.text; }
                private string TextInput { get => valueInput.text; }
                private bool ToggleValue { get => boolInput.isOn; }

                private LocalObjectUI parent;
                public LocalObjectUI Parent { get => parent; set => parent = value; }

                private int id = 0;
                public int ID { get => id; set => id = value; }

                public void OnChangeType()
                {
                    if (Type == ValueType.BOOL) SwitchToState(UIState.BOOL);
                    else SwitchToState(UIState.INPUT);

                    switch (Type)
                    {
                        case ValueType.BOOL:
                            break;

                        case ValueType.STRING:
                            valueInput.contentType = InputField.ContentType.Standard;
                            break;

                        case ValueType.DOUBLE:
                            valueInput.contentType = InputField.ContentType.DecimalNumber;
                            break;

                        case ValueType.FLOAT:
                            valueInput.contentType = InputField.ContentType.DecimalNumber;
                            break;

                        case ValueType.INTEGER:
                            valueInput.contentType = InputField.ContentType.IntegerNumber;
                            break;

                        default:
                            valueInput.contentType = InputField.ContentType.Standard;
                            break;
                    }
                }

                public void OnChangeToggle()
                {
                    toggleText.text = ToggleValue ? "True" : "False";
                }

                private void SwitchToState(UIState s)
                {
                    if (s == State) return;

                    switch (s)
                    {
                        case UIState.BOOL:
                            boolInput.gameObject.SetActive(true);
                            valueInput.gameObject.SetActive(false);
                            state = s;
                            break;

                        case UIState.INPUT:
                            boolInput.gameObject.SetActive(false);
                            valueInput.gameObject.SetActive(true);
                            state = s;
                            break;
                    }
                }

                public void OnClickRemove()
                {
                    parent.OnRemoveVariable(id);
                    Destroy(gameObject);
                }

                public Property GetProperty()
                {
                    switch (Type)
                    {
                        case ValueType.BOOL:
                            return new Property(Name, ToggleValue, Type);

                        case ValueType.STRING:
                            return new Property(Name, TextInput, Type);

                        case ValueType.DOUBLE:
                            return new Property(Name, double.Parse(TextInput), Type);

                        case ValueType.FLOAT:
                            return new Property(Name, float.Parse(TextInput), Type);

                        case ValueType.INTEGER:
                            return new Property(Name, int.Parse(TextInput), Type);

                        case ValueType.OBJECT:
                            return new Property(Name, TextInput, Type);

                        default:
                            return new Property(Name, TextInput, ValueType.INVALID);
                    }
                }

                public VariableUITemplate Template { get => new VariableUITemplate(Name, TextInput, Type, ToggleValue, (int)state); }

                public void LoadFromTemplate(VariableUITemplate template)
                {
                    SwitchToState((UIState)template.state);
                    nameInput.text = template.name;
                    valueInput.text = template.value;
                    boolInput.isOn = template.boolean;
                    typeDropdown.value = (int)template.type;
                }
            }
        }
    }
}