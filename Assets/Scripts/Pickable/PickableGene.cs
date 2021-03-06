using UnityEngine;
using System.Collections;

public class PickableGene : PickableBioBrick {
  public string bioBrickName;
  public string proteinName;

    protected override DNABit produceDNABit()
  {
    return new GeneBrick(bioBrickName, proteinName);
  }
}
