using Blabbeur.Grammar;
using Blabbeur.Objects;
using Blabbeur.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;


namespace Blabbeur
{
    /// <summary>
    /// The TextGen Singleton is the main interface for Blabbeur. Here, global objects and grammars are stored, and requests can be made.
    /// </summary>
    /// 
    
    public class TextGen : GenericSingletonClass<TextGen>
    {
        #region Variables and Initialization

        public const int MAXDEPTH = 10;
        public const string MAXDEPTHERROR = "<<Error, Max Depth Reached!>>";
        private const string ERRORSTRING = "ERROR"; //Placeholder

        /// <summary>
        /// A list of text asset grammar files that can be added in Unity
        /// </summary>
        public TextAsset[] grammarFiles;

        /// <summary>
        /// A list of property behaviours that can be added in Unity
        /// </summary>
        public PropertyBehaviour[] GlobalObjects;

        /// <summary>
        /// A list of all the grammars assigned to Blabbeur
        /// </summary>
        private static Dictionary<string, BlabbeurGrammar> grammars;

        public static IEnumerable grammarNames { get => grammars.Keys; }
        public static IEnumerable grammarObjects { get => grammars.Values; }

        /// <summary>
        /// A list of all the global objects assigned to Blabbeur
        /// </summary>
        private static Dictionary<string, BlabbeurObject> globalObjects;

        public static IEnumerable globalObjectNames { get => globalObjects.Keys; }
        public static IEnumerable globalObjectList { get => globalObjects.Values; }

        /// <summary>
        /// Check if a particular grammar exists
        /// </summary>
        /// <param name="grammarName">The name of the grammar</param>
        /// <returns>True if the grammar exists</returns>
        public bool HaveGrammar(string grammarName) => grammars.ContainsKey(grammarName);

        /// <summary>
        /// Check if a particular global object exists
        /// </summary>
        /// <param name="objectName">The name of the global object</param>
        /// <returns>True if the global object exists</returns>
        public bool HaveGlobalObject(string objectName) => globalObjects.ContainsKey(objectName);

        /// <summary>
        /// The active grammar being used by the text generator for generation
        /// </summary>
        private static FlagValue<string> activeGrammar;

        public static BlabbeurGrammar ActiveGrammar
        {
            get
            {
                if (activeGrammar.flag) return grammars[activeGrammar.value];
                else throw new System.Exception("Attempted to access grammar when none is active");
            }
            set => activeGrammar.Set(value.Name, true);
        }

        /// <summary>
        /// The active local object being used by the text generator for generation
        /// </summary>
        private static FlagValue<BlabbeurObject> activeLocalObject;

        /// <summary>
        /// The set up performed after the singleton is initialize
        /// </summary>
        protected override void AfterAwake()
        {
            //Set everything to default
            grammars = new Dictionary<string, BlabbeurGrammar>();

            globalObjects = new Dictionary<string, BlabbeurObject>();

            //Load an property behaviours that were assigned in Unity
            for (int i = 0; i < GlobalObjects.Length; i++) RegisterGlobalObject(GlobalObjects[i]);

            activeGrammar = new FlagValue<string>("", false);
            activeLocalObject = new FlagValue<BlabbeurObject>(null, false);

            //Load all the grammar files
            for (int i = 0; i < grammarFiles.Length; i++)
                AddGrammar(grammarFiles[i]);
        }

        static readonly object pblock = new object();

        #endregion Variables and Initialization

        #region Load Functions

        /// <summary>
        /// Load a blabbeur grammar from a given filename
        /// </summary>
        /// <param name="filename">The name of the file to load</param>
        /// <returns>A Blabbeur grammar</returns>
        public static BlabbeurGrammar LoadFromFile(string filename)
        {
            using (StreamReader reader = File.OpenText(filename))
                return LoadFromText(reader.ReadToEnd());
        }

        /// <summary>
        /// Load a blabbeur grammar from a text
        /// </summary>
        /// <param name="text">The text representing the full a grammar</param>
        /// <returns>A Blabbeur Grammar</returns>
        public static BlabbeurGrammar LoadFromText(string text) => Parser.LoadBlabbeurGrammar(text);

        #endregion Load Functions

        #region Grammar Management

        /// <summary>
        /// Add a new grammar from a text asset
        /// </summary>
        /// <param name="grammar">The text asset to be loaded</param>
        public void AddGrammar(TextAsset grammar) => AddGrammar(LoadFromText(grammar.text));

        /// <summary>
        /// Add a new grammar
        /// </summary>
        /// <param name="grammar">The grammar to be added</param>
        public void AddGrammar(BlabbeurGrammar grammar)
        {
            if (!grammars.ContainsKey(grammar.Name))
                grammars[grammar.Name] = grammar;
            else
                Debug.LogWarning("Grammar with the same name already exists! Operation aborted");
        }

        /// <summary>
        /// Remove a specific grammar
        /// </summary>
        /// <param name="grammar">The grammar to be removed</param>
        public void RemoveGrammar(BlabbeurGrammar grammar) => RemoveGrammar(grammar.Name);

        /// <summary>
        /// Remove a specific grammar
        /// </summary>
        /// <param name="grammarName">The name of the grammar to be removed</param>
        public void RemoveGrammar(string grammarName)
        {
            if (grammars.ContainsKey(grammarName))
                grammars.Remove(grammarName);
        }

        #endregion Grammar Management

        #region Global Object Management

        /// <summary>
        /// Register a new global object in Blabbeur
        /// </summary>
        /// <param name="gObject">The object to be added</param>
        public void RegisterGlobalObject(BlabbeurObject gObject) => globalObjects[gObject.ID] = gObject;

        /// <summary>
        /// Deregister a global object in Blabbeur
        /// </summary>
        /// <param name="gObject">The object to be deregistered</param>
        public void DeregisterGlobalObject(BlabbeurObject gObject)
        {
            if (globalObjects.ContainsKey(gObject.ID))
                globalObjects.Remove(gObject.ID);
        }

        /// <summary>
        /// Deregister a global object in Blabbeur
        /// </summary>
        /// <param name="gObject">The name of the object to be deregistered</param>
        public void DeregisterGlobalObject(string id)
        {
            if (globalObjects.ContainsKey(id))
                globalObjects.Remove(id);
        }

        #endregion Global Object Management

        #region Request Functions

        /// <summary>
        /// Called on the end of a request
        /// </summary>
        private static void endRequest()
        {
            //Reset the grammar and local object
            activeGrammar.Set("", false);
            activeLocalObject.Set(null, false);
        }

        /// <summary>
        /// Request a specific object from Blabbeur. Checks global and local objects
        /// </summary>
        /// <param name="search">The search string</param>
        /// <returns>The referenced object</returns>
        public static BlabbeurObject RequestObject(string[] search)
        {
            if (search.Length > 1)
            {
                int start = 0;
                BlabbeurObject activeObject;

                //See if the first search term is the name of a global object
                if (globalObjects.ContainsKey(search[0]))
                {
                    activeObject = globalObjects[search[0]];
                    start = 1;
                }

                //Otherwise see if a local object is available it is returned
                else if (activeLocalObject.flag) activeObject = activeLocalObject.value;
                else throw new System.Exception(string.Format("Cannot find a parameter or global object with the value {0}", search[0]));

                //Recursively get the next object for recursive searches
                for (int i = start; i < search.Length - 1; i++)
                    activeObject = activeObject.RequestObject(search[i]);

                return activeObject;
            }
            else if (search.Length == 1 && activeLocalObject.flag) return activeLocalObject.value;
            else throw new System.Exception("Cannot search for an empty parameter!");
        }

        /// <summary>
        /// Request an expansion of a specific grammar
        /// </summary>
        /// <param name="grammarName">The name of the grammar</param>
        /// <returns>An expansion of the requested grammar</returns>
        public static string Request(string grammarName)
        {
            if (grammars.ContainsKey(grammarName))
            {
                ActiveGrammar = grammars[grammarName];
                string text = ActiveGrammar.Expand();

                endRequest();
                return text;
            }
            else throw new System.Exception(string.Format("Error, attempting a request from a grammar,{0}, which does not exist", grammarName));
        }

        /// <summary>
        /// Request an expansion of a specific grammar and a specific symbol
        /// </summary>
        /// <param name="grammarName">The name of the grammar</param>
        /// <param name="symbol">The symbol to expand</param>
        /// <returns>An expansion of the requested grammar</returns>
        public static string Request(string grammarName, string symbol)
        {
            if (grammars.ContainsKey(grammarName))
            {
                ActiveGrammar = grammars[grammarName];
                string text = ActiveGrammar.Expand(symbol);

                endRequest();
                return text;
            }
            else throw new System.Exception(string.Format("Error, attempting a request from a grammar,{0}, which does not exist", grammarName));
        }

        /// <summary>
        /// Request an expansion of a specific grammar with a provided local object
        /// </summary>
        /// <param name="grammarName">The name of the grammar</param>
        /// <param name="localObject">The local object</param>
        /// <returns>An expansion of the requested grammar</returns>
        public static string Request(string grammarName, BlabbeurObject localObject)
        {
            //lock (pblock)
           // {
                if (grammars.ContainsKey(grammarName))
                {
                    ActiveGrammar = grammars[grammarName];
                    activeLocalObject.Set(localObject);
                    string text = ActiveGrammar.Expand();

                    endRequest();
                    return text;
                }
                else throw new System.Exception(string.Format("Error, attempting a request from a grammar,{0}, which does not exist", grammarName));
           // }
 
        }

        /// <summary>
        /// Request an expansion of a specific grammar and a specific symbol, with a provided local object
        /// </summary>
        /// <param name="grammarName">The name of the grammar</param>
        /// <param name="symbol">The symbol to expand</param>
        /// <param name="localObject">The local object</param>
        /// <returns>An expansion of the requested grammar</returns>
        public static string Request(string grammarName, string symbol, BlabbeurObject localObject)
        {
            if (grammars.ContainsKey(grammarName))
            {
                ActiveGrammar = grammars[grammarName];
                activeLocalObject.Set(localObject);
                string text = ActiveGrammar.Expand(symbol);

                endRequest();
                return text;
            }
            else throw new System.Exception(string.Format("Error, attempting a request from a grammar,{0}, which does not exist", grammarName));
        }

        /// <summary>
        /// Log a warning in blabbeur, outputs the result in the Unity debug window
        /// </summary>
        /// <param name="warning">The warning to throw</param>
        public static void LogWarning(string warning) => Debug.LogWarning(warning);

        #endregion Request Functions
    }
}