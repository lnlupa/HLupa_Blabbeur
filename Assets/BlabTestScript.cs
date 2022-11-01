using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlabTestScript : MonoBehaviour
{
    //blabbeur specific object that holds blabbeur generated material in a dictionary
    //one of these will be created for each character
    Blabbeur.Objects.PropertyDictionary chara = new Blabbeur.Objects.PropertyDictionary("character");

    //another dictionary that holds the dictionaries of all the characters
    //hopefully in the future I will figure out how to access these from regular Unity code
    Blabbeur.Objects.PropertyDictionary data = new Blabbeur.Objects.PropertyDictionary("data");

    //text to display the info onscreen
    public TMP_Text guyText;

    public TMP_Text guyCount;

    int numAttendees = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space)) 
        {
            chara.Add("name", Blabbeur.TextGen.Request("InventAPerson"));
            chara.Add("pronoun", Blabbeur.TextGen.Request("Pronouns"));
            chara.Add("age", Blabbeur.TextGen.Request("Age"));
            chara.Add("job", Blabbeur.TextGen.Request("Job"));
            chara.Add("popularity", Blabbeur.TextGen.Request("Popularity"));
            chara.Add("bestfriend", Blabbeur.TextGen.Request("Bestfriend"));
            string output = chara["name"].ToString();
            Debug.Log(output);

            data.Add("character", chara);

        }

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            string outp = Blabbeur.TextGen.Request("PhraseAPerson", chara);
            Debug.Log(outp);
            guyText.SetText(outp);
        }*/
    }

    //function that acccesses the blabbeur grammars and requests a random result for each of the pieces of info we want about our guys
    public void makeAGuy() 
    {
        chara.Add("name", Blabbeur.TextGen.Request("InventAPerson"));
        chara.Add("pronoun", Blabbeur.TextGen.Request("Pronouns"));
        chara.Add("age", Blabbeur.TextGen.Request("Age"));
        chara.Add("job", Blabbeur.TextGen.Request("Job"));
        chara.Add("popularity", Blabbeur.TextGen.Request("Popularity"));
        chara.Add("bestfriend", Blabbeur.TextGen.Request("Bestfriend"));
        string output = chara["name"].ToString();
        //Debug.Log(output);
        guyText.SetText(output);
        data.Add("character", chara);
        numAttendees++;

        //updating the count of attendees every
        guyCount.SetText(numAttendees.ToString());
    }

    //function to call the blabbeur grammar that writes the various sentences of info about each guy
    public void displayGuy()
    {
        string outp = Blabbeur.TextGen.Request("PhraseAPerson", chara);
        //Debug.Log(outp);
        guyText.SetText(outp);
    }

}
