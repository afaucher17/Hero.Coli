using UnityEngine;
using System;
using System.Xml;

/*!
  \brief This class describe a Molecule
  \details A molecule is define by :
        - A name -> Used for identification
        - A type (Enzyme, transcription factor or other)
        - A description (optionnal)
        - A concentration
        - A degradation rate -> used for degradation reaction
        - A Size -> used for fick reaction (not implemented yet)


Molecules which are declared in files should respect this syntax :

      <molecule type="other">
        <name>H2O</name>
        <description>de l'eau!</description>
        <concentration>0</concentration>
        <degradationRate>0.013</degradationRate>
        <FickFactor>0.33</FickFactor>
      </molecule>
 */
public class Molecule : LoadableFromXmlImpl
{
  //! Define Molecule type
  public enum eType
  {
    ENZYME,
    TRANSCRIPTION_FACTOR,
    OTHER
  }

  private string _name;                           //!< The use name of the molecule
  private string _realName = null;                //!< The real name of the molecule
  private eType _type;                            //!< The type of the molecule
  private string _description;                    //!< The description of the molecule (optionnal)
  private float _concentration;                   //!< The concentration of the molecule
  private float _newConcentration;                //!< The concentration of the molecule for the next stage
  private float _degradationRate;                 //!< The degradation rate of the molecule
  private float _fickFactor;                      //!< The FickFactor is a coefficient for FickReaction
  private float _negligibilityThreshold = 1E-10f; //!< The threshold below which the concentration is rounded down to 0.

  public override string getTag() { return "molecule"; }

  private bool _debug = false;

  //! Default constructor
    public Molecule() : this(null) {}

  public Molecule(Molecule mol = null)
  {
    if (mol != null)
    {
      if(_debug) Logger.Log("Molecule::Molecule("+mol+")", Logger.Level.TRACE);
      setName(mol._name);
      _type = mol._type;
      _description = mol._description;
      _concentration = mol._concentration;
      _degradationRate = mol._degradationRate;
      _fickFactor = mol._fickFactor;
      _newConcentration = mol._newConcentration;
      if(_debug) Logger.Log("Molecule::Molecule("+mol+") built "+this, Logger.Level.TRACE);
    } else {
      if(_debug) Logger.Log("Molecule::Molecule(null)", Logger.Level.TRACE);
    }
  }

  public string getName() {return _name; }
  public string getRealName() {return _realName; }
  public eType getType() {return _type; }
  public string getDescription() {return _description; }
  public float getConcentration() {
      if(_debug) Logger.Log ("Molecule::getConcentration "+_name+" "+_concentration, Logger.Level.TRACE);
      return _concentration;
  }
  public float getDegradationRate() {return _degradationRate; }
  public float getFickFactor() { return _fickFactor; }
  public void setName(string name)
  {
    _name = name;
    _realName = GameplayNames.getMoleculeRealName(_name);
  }    
  public void OnLanguageChanged()
  {
    _realName = GameplayNames.getMoleculeRealName(_name);
  }
  public void setType(eType type) { _type = type; }
  public void setDescription(string description) { _description = description; }
  public void setConcentration(float concentration) {
      float oldConcentration = _concentration;
      _concentration = concentration; if (_concentration < _negligibilityThreshold) _concentration = 0;
      if(_debug) Logger.Log("Molecule::setConcentration("+concentration+") "+_name+" old="+oldConcentration+", new="+_concentration, Logger.Level.TRACE);
  }
  public void setDegradationRate(float degradationRate) { _degradationRate = degradationRate; }
  public void addNewConcentration(float concentration) {
      float oldNewCC = _newConcentration;
      _newConcentration += concentration; if (_newConcentration < _negligibilityThreshold) _newConcentration = 0;
      if(_debug) Logger.Log("Molecule::addNewConcentration("+concentration+") "+_name+" oldNewCC="+oldNewCC+", new="+_newConcentration, Logger.Level.TRACE);
  }
  public void subNewConcentration(float concentration) {
      float oldNewCC = _newConcentration;
      _newConcentration -= concentration; if (_newConcentration < _negligibilityThreshold) _newConcentration = 0;
      if(_debug) Logger.Log("Molecule::subNewConcentration("+concentration+") "+_name+" oldNewCC="+oldNewCC+", new="+_newConcentration, Logger.Level.TRACE);
  }
  public void setNewConcentration(float concentration) {
      float oldNewCC = _newConcentration;
      _newConcentration = concentration; if (_newConcentration < _negligibilityThreshold) _newConcentration = 0;
      if(_debug) Logger.Log("Molecule::setNewConcentration("+concentration+") "+_name+" old="+oldNewCC+", new="+_newConcentration, Logger.Level.TRACE);
  }
  public void setFickFactor(float v) { _fickFactor = v; }

  /*!
  \brief Add molecule concentration
  \param concentration The concentration
  */
  public void addConcentration(float concentration) {
      float oldCC = _concentration;
      _concentration += concentration; if (_concentration < _negligibilityThreshold) _concentration = 0;
      if(_debug) Logger.Log("Molecule::addConcentration("+concentration+") "+_name+" old="+oldCC+", new="+_concentration, Logger.Level.TRACE);
  }

  /*!
  \brief Add molecule concentration
  \param concentration The concentration
  */
  public void subConcentration(float concentration) {
      float oldCC = _concentration;
      _concentration -= concentration; if (_concentration < _negligibilityThreshold) _concentration = 0;
      if(_debug) Logger.Log("Molecule::subConcentration("+concentration+") "+_name+" old="+oldCC+", new="+_concentration, Logger.Level.TRACE);
  }

  //! \brief This function set the actual concentration to it new value
  public void updateConcentration() {
      if(_debug) Logger.Log("Molecule::updateConcentration() "+_name+" old="+_concentration+", new="+_newConcentration, Logger.Level.TRACE);
      _concentration = _newConcentration;
  }

  public override bool tryInstantiateFromXml(XmlNode moleculeNode)
  {
      Logger.Log ("Molecule.tryInstantiateFromXml("+Logger.ToString(moleculeNode)+")", Logger.Level.INFO);
      
      if (moleculeNode.Name == getTag())
      {
          if(null != moleculeNode.Attributes["type"])
          {
              Molecule.eType type = Molecule.eType.OTHER;
              
              switch (moleculeNode.Attributes["type"].Value)
              {
                case "enzyme":
                {
                  type = Molecule.eType.ENZYME;
                  break;
                }
                case "transcription_factor":
                {
                  type = Molecule.eType.TRANSCRIPTION_FACTOR;
                  break;
                }
                case "other":
                {
                  type = Molecule.eType.OTHER;
                  break;
                }
                //TODO add this case to all tryInstantiateFromXml implementations
                case XMLTags.COMMENT:
                  break;
                default:
                {
                  Logger.Log ("Molecule::tryInstantiateFromXml unknown molecule type "+moleculeNode.Attributes["type"].Value
                              ,Logger.Level.WARN);
                  return false;
                }
              }
              
              setType(type);
              
              foreach (XmlNode attr in moleculeNode)
              {
                  switch (attr.Name)
                  {
                      case "name":
                          setName(attr.InnerText);
                          break;
                      case "description":
                          setDescription(attr.InnerText);
                          break;
                      case "concentration":
                          setConcentration(float.Parse(attr.InnerText.Replace(",", ".")));
                          break;
                      case "degradationRate":
                          setDegradationRate(float.Parse(attr.InnerText.Replace(",", ".")));
                          break;
                      case "FickFactor":
                          setFickFactor(float.Parse(attr.InnerText.Replace(",", ".")));
                          break;
                      case XMLTags.COMMENT:
                          break;
                      default:
                          Logger.Log ("Molecule.tryInstantiateFromXml("+Logger.ToString(moleculeNode)+", loader) finished early"
                                      +" - unknown attribute "+attr.Name
                                      , Logger.Level.WARN);
                          return false;
                  }
              }

              if(
                    string.IsNullOrEmpty(_name)                           //!< The use name of the molecule
                    || string.IsNullOrEmpty(_realName)                    //!< The real name of the molecule
                    || (null == _type)                                    //!< The type of the molecule
                    //private string _description;                        //!< The description of the molecule (optionnal)
                    //private float _concentration;                       //!< The concentration of the molecule
                    //private float _newConcentration;                    //!< The concentration of the molecule for the next stage
                    //private float _degradationRate;                     //!< The degradation rate of the molecule
                    //private float _fickFactor;                          //!< The FickFactor is a coefficient for FickReaction
                    )
                {
                  Logger.Log ("Molecule.tryInstantiateFromXml("+Logger.ToString(moleculeNode)+", loader) failed eventually because "
                                +"_name="+_name
                                +"& _realName="+_realName
                                +"& _type="+_type
                                , Logger.Level.ERROR);
                  return false;
                }
                else
                {
              
                  Logger.Log ("Molecule.tryInstantiateFromXml("+Logger.ToString(moleculeNode)+", loader) finished"
                          +" with molecule="+this
                            , Logger.Level.DEBUG);
                  return true;
                }
          }
          else
          {
              Logger.Log ("Molecule.tryInstantiateFromXml("+Logger.ToString(moleculeNode)+", loader) finished early"
                          +"- no type in "+Logger.ToString(moleculeNode)
                          , Logger.Level.WARN);
              return false;
          }
      }
      else
      {
          Logger.Log("Molecule.tryInstantiateFromXml bad name in "+Logger.ToString(moleculeNode), Logger.Level.WARN);
          return false;
      }
  }

  public override string ToString() {
      return "Molecule[name:"+_name
          +", t:"+_type
              +", d:"+_description
              +", cc:"+_concentration
              +", ncc:"+_newConcentration
              +", dr:"+_degradationRate
              +", ff:"+_fickFactor
              +"]";
  }
  public string ToShortString(bool displayAll) {
      if (!displayAll && _concentration == 0) {
          return null;
      } else {
          return _realName+":"+_concentration;
      }
  }
}

