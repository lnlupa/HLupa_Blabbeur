using Blabbeur.Objects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blabbeur
{
    namespace Unity
    {
        namespace UI
        {
            public struct VariableUITemplate
            {
                public string name;
                public string value;
                public ValueType type;
                public bool boolean;
                public int state;

                public VariableUITemplate(string _name, string _value, ValueType _type, bool _bool, int _state)
                {
                    name = _name;
                    value = _value;
                    type = _type;
                    boolean = _bool;
                    state = _state;
                }
            }

            public struct LocalObjectTemplate
            {
                public string name;
                public List<VariableUITemplate> variables;

                public LocalObjectTemplate(string _name)
                {
                    name = _name;
                    variables = new List<VariableUITemplate>();
                }
            }

            public class TestUI : MonoBehaviour
            {
                public Dropdown grammarList;
                public InputField grammarSearch;
                private List<string> grammarOptions;
                private string SelectedGrammar { get => grammarOptions[grammarList.value]; }

                public LocalObjectUI localObjectUI;
                public List<LocalObjectTemplate> localObjects;

                public InputField amountToGenerate;
                public Text output;

                private int activeLocalObject;

                public List<PropertyBehaviour> variablesToLoad;

                // Start is called before the first frame update
                private void Start()
                {
                    grammarOptions = new List<string>();
                    localObjects = new List<LocalObjectTemplate>();
                    activeLocalObject = -1;

                    SetActiveObject(activeLocalObject);
                    LoadGrammars();
                    LoadPropertyBehaviours();
                }

                private void LoadPropertyBehaviours()
                {
                    if (variablesToLoad.Count > 0)
                    {
                        for (int i = 0; i < variablesToLoad.Count; i++)
                        {
                            LocalObjectTemplate t = new LocalObjectTemplate(variablesToLoad[i].name);

                            foreach (UnityProperty property in variablesToLoad[i].properties)
                            {
                                Property p = property.ToProperty();
                                t.variables.Add(new VariableUITemplate(p.Name, p.Value.ToString(), p.Type, p.Type == ValueType.BOOL ? (bool)p.Value : false, p.Type == ValueType.BOOL ? 0 : 1));
                            }

                            foreach (UnityBlabbeurObjectProperty property in variablesToLoad[i].blabbeurObjectProperties)
                            {
                                Property p = property.ToProperty();
                                t.variables.Add(new VariableUITemplate(p.Name, ((BlabbeurObject)p.Value).ID, p.Type, false, 1));
                            }

                            localObjects.Add(t);
                        }
                        SetActiveObject(0);
                    }
                }

                public void OnGrammarSearchUpdated() => LoadGrammars();

                private void LoadGrammars()
                {
                    grammarOptions.Clear();
                    grammarList.ClearOptions();

                    foreach (string name in TextGen.grammarNames)
                    {
                        if (!string.IsNullOrEmpty(grammarSearch.text))
                        {
                            if (name.Contains(grammarSearch.text))
                                grammarOptions.Add(name);
                        }
                        else
                            grammarOptions.Add(name);
                    }
                    grammarList.AddOptions(grammarOptions);
                }

                public void OnClickNewLocalObject()
                {
                    if (localObjects.Count > 0) NewActiveObject(activeLocalObject + 1);
                    else NewActiveObject(0);
                }

                private void NewActiveObject(int pos)
                {
                    localObjects.Insert(pos, new LocalObjectTemplate(""));
                    SetActiveObject(pos);
                }

                public void OnSwapActiveObject(int direction)
                {
                    int newPos = activeLocalObject + direction;

                    if (newPos > -1)
                    {
                        if (newPos < localObjects.Count)
                            SetActiveObject(newPos);
                    }
                }

                public void OnRemoveActiveObject()
                {
                    if (activeLocalObject > -1)
                    {
                        int newPos = activeLocalObject - 1;
                        SetActiveObject(newPos);
                        localObjects.RemoveAt(newPos + 1);
                    }
                }

                private void SetActiveObject(int id)
                {
                    StoreActiveLocalObject();

                    if (id > -1 && id < localObjects.Count)
                        localObjectUI.LoadTemplate(localObjects[id]);
                    else
                        localObjectUI.RemoveObject();

                    activeLocalObject = id;
                }

                private void StoreActiveLocalObject()
                {
                    if (activeLocalObject > -1)
                        localObjects[activeLocalObject] = localObjectUI.ToTemplate();
                }

                private int SearchForObject(string name, PropertyDictionary[] dict)
                {
                    for (int i = 0; i < dict.Length; i++)
                        if (dict[i].ID.Equals(name))
                            return i;
                    return -1;
                }

                public PropertyDictionary[] GetBlabbeurObjects()
                {
                    StoreActiveLocalObject();
                    PropertyDictionary[] blabbeurObjects = new PropertyDictionary[localObjects.Count];

                    //We do this in two stages to resolve object references
                    for (int i = 0; i < localObjects.Count; i++)
                        blabbeurObjects[i] = new PropertyDictionary(localObjects[i].name);

                    for (int j = 0; j < localObjects.Count; j++)
                    {
                        foreach (VariableUITemplate variable in localObjects[j].variables)
                        {
                            switch (variable.type)
                            {
                                case ValueType.BOOL:
                                    blabbeurObjects[j].Add(new Property(variable.name, variable.boolean, ValueType.BOOL));
                                    break;

                                case ValueType.STRING:
                                    blabbeurObjects[j].Add(new Property(variable.name, variable.value, ValueType.STRING));
                                    break;

                                case ValueType.DOUBLE:
                                    blabbeurObjects[j].Add(new Property(variable.name, double.Parse(variable.value), ValueType.DOUBLE));
                                    break;

                                case ValueType.FLOAT:
                                    blabbeurObjects[j].Add(new Property(variable.name, float.Parse(variable.value), ValueType.FLOAT));
                                    break;

                                case ValueType.INTEGER:
                                    blabbeurObjects[j].Add(new Property(variable.name, int.Parse(variable.value), ValueType.INTEGER));
                                    break;

                                case ValueType.OBJECT:
                                    int id = SearchForObject(variable.value, blabbeurObjects);
                                    if (id > -1)
                                        blabbeurObjects[j].Add(new Property(variable.name, blabbeurObjects[id], ValueType.OBJECT));
                                    else
                                        blabbeurObjects[j].Add(new Property(variable.name, variable.value, ValueType.OBJECT));
                                    break;

                                default:
                                    blabbeurObjects[j].Add(new Property(variable.name, variable.value, ValueType.INVALID));
                                    break;
                            }
                        }
                    }

                    return blabbeurObjects;
                }

                public void OnClickBlab()
                {
                    PropertyDictionary[] obj = GetBlabbeurObjects();
                    int numRequests = !string.IsNullOrEmpty(amountToGenerate.text) ? int.Parse(amountToGenerate.text) : 1;
                    string outputText = "";
                    if (activeLocalObject > -1)
                    {
                        for (int i = 0; i < numRequests; i++)
                            outputText = string.Format("{0}~ {1}\n", outputText, TextGen.Request(SelectedGrammar, obj[activeLocalObject]));
                    }
                    else
                    {
                        for (int i = 0; i < numRequests; i++)
                            outputText = string.Format("{0}~ {1}\n", outputText, TextGen.Request(SelectedGrammar));
                    }
                    output.text = outputText;
                }
            }
        }
    }
}