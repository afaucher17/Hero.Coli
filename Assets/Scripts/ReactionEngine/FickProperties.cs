using UnityEngine;
using System.Xml;
using System;

//!  
/*!
  \brief     Describe a FickReaction
  \details   This class is a descriptive class of a FickReaction
 A fick reaction file should respect this syntax :
 
     <ficks>
      <fickProp>
        <MediumId1>2</MediumId1>                -> Unique ID of the first medium
        <MediumId2>1</MediumId2>                -> Unique ID of the second medium
        <P>0.05</P>                             -> Permeability coefficient
        <surface>0.003</surface>                -> contact surface between the two mediums
      </fickProp>
     </ficks>
 */


public class FickProperties : LoadableFromXmlImpl
{
  public int MediumId1 {get; set;}
  public int MediumId2  {get; set;}
  public float P  {get; set;}
  public float surface  {get; set;}
  public float energyCost {get; set;}

  //! Create from an XML node a FickProperties.
  //! \param node The XML node
    public override bool tryInstantiateFromXml(XmlNode node)
  {
        Logger.Log("FickProperties.tryInstantiateFromXml("+node+") will load", Logger.Level.DEBUG);

    foreach (XmlNode attr in node)
    {
      switch (attr.Name)
      {
        case "MediumId1":
          if(String.IsNullOrEmpty(attr.InnerText))
          {
              Logger.Log("FickProperties::tryInstantiateFromXml empty MediumId1"
                                 , Logger.Level.ERROR);
              return false;
          }
          else
          {
              MediumId1 = Convert.ToInt32(attr.InnerText);
          }          
          break;
        case "MediumId2":
          if(String.IsNullOrEmpty(attr.InnerText))
          {
              Logger.Log("FickProperties::tryInstantiateFromXml empty MediumId2"
                         , Logger.Level.ERROR);
              return false;
          }
          else
          {
              MediumId2 = Convert.ToInt32(attr.InnerText);
          }
          break;
        case "P":
          P = float.Parse(attr.InnerText.Replace(",", "."));
          break;
        case "surface":
          surface = float.Parse(attr.InnerText.Replace(",", "."));
          break;
        default:
          Logger.Log ("FickProperties::tryInstantiateFromXml(node) unexpected attribute "+attr.Name, Logger.Level.ERROR);
          return false;
      }
    }

        Logger.Log("FickProperties.tryInstantiateFromXml(node) loaded this="+this, Logger.Level.DEBUG);
    return true;
  }
}