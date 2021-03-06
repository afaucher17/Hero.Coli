﻿using Vectrosity;

public abstract class VectrosityPanelLine {

    public string name; //!< The line name
    public string moleculeName; //!< The name of the molecule whose concentration is represented by this VectrosityPanelLine

    public abstract VectorLine vectorline {get;} //!< The Vectrosity line
    public abstract void resize();
    public abstract void redraw();
    public abstract void addPoint(float value);
	public abstract void doDebugAction();
    
    public virtual void setActive(bool isActive) {
        vectorline.active = isActive;
    }    
    public virtual void destroyLine() {}
    public virtual void initializeVectorLine() {}

    public static string generateLineName(int _mediumId, string _moleculeName) {
        return _mediumId+"."+_moleculeName;
    }
}
