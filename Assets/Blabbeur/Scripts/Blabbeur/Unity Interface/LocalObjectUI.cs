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
            public class LocalObjectUI : MonoBehaviour
            {
                #region Variables and Initialization

                public InputField localObjectName;

                public GameObject variablePanelPrefab;
                public GameObject variableContent;

                public GameObject variableScrollview;
                private List<VariableUI> localVariables;

                private List<string> localObjectNames;

                private bool active;

                private void Start()
                {
                    localVariables = new List<VariableUI>();
                }

                private void SetActive(bool _active)
                {
                    localObjectName.gameObject.SetActive(_active);
                    variableScrollview.SetActive(_active);
                    active = _active;
                }

                #endregion Variables and Initialization

                #region Property Dictionary Generatation

                public PropertyDictionary ToLocalObject()
                {
                    PropertyDictionary localObject = new PropertyDictionary(localObjectName.text);

                    for (int i = 0; i < localVariables.Count; i++)
                        localObject.Add(localVariables[i].GetProperty());

                    return localObject;
                }

                #endregion Property Dictionary Generatation

                #region Variable List Management

                public void UpdateIDs()
                {
                    for (int i = 0; i < localVariables.Count; i++)
                        localVariables[i].ID = i;
                }

                private void Refresh()
                {
                    localObjectName.text = "";
                    localVariables.Clear();
                    ClearList();
                }

                private void ClearList()
                {
                    for (int i = 0; i < variableContent.transform.childCount - 1; i++)
                        Destroy(variableContent.transform.GetChild(i).gameObject);
                }

                private void AddVariable(VariableUI ui)
                {
                    localVariables.Add(ui);
                    UpdateIDs();
                }

                public void OnRemoveVariable(int id)
                {
                    localVariables.RemoveAt(id);
                    UpdateIDs();
                }

                #endregion Variable List Management

                #region New Variable Creation

                public void OnClickNewVariable() => CreateNewVariable();

                private void CreateNewVariable()
                {
                    GameObject newVariable = Instantiate(variablePanelPrefab, variableContent.transform);
                    VariableUI newVarUI = newVariable.GetComponent<VariableUI>();
                    newVarUI.transform.SetSiblingIndex(variableContent.transform.childCount - 2);
                    newVarUI.Parent = this;

                    AddVariable(newVarUI);
                }

                #endregion New Variable Creation

                #region New Local Object

                public void NewLocalObject()
                {
                    SetActive(true);
                    Refresh();
                }

                public void RemoveObject()
                {
                    SetActive(false);
                    Refresh();
                }

                #endregion New Local Object

                #region Template Saving/Loading

                public void LoadTemplate(LocalObjectTemplate template)
                {
                    SetActive(true);
                    Refresh();
                    localObjectName.text = template.name;
                    foreach (VariableUITemplate variable in template.variables)
                        CreateVariableFromTemplate(variable);
                }

                private void CreateVariableFromTemplate(VariableUITemplate template)
                {
                    GameObject newVariable = Instantiate(variablePanelPrefab, variableContent.transform);
                    VariableUI newVarUI = newVariable.GetComponent<VariableUI>();
                    newVarUI.transform.SetSiblingIndex(variableContent.transform.childCount - 2);
                    newVarUI.Parent = this;
                    newVarUI.LoadFromTemplate(template);

                    AddVariable(newVarUI);
                }

                public LocalObjectTemplate ToTemplate()
                {
                    LocalObjectTemplate template = new LocalObjectTemplate(localObjectName.text);
                    foreach (VariableUI variable in localVariables)
                        template.variables.Add(variable.Template);
                    return template;
                }

                #endregion Template Saving/Loading
            }
        }
    }
}