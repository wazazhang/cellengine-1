package com.g2d.studio.ui.edit.gui;

import org.w3c.dom.Element;

import com.g2d.display.ui.TextPan;
import com.g2d.studio.ui.edit.UIEdit;

public class UETextBox extends TextPan implements SavedComponent
{
	public UETextBox() {
	}
	
	@Override
	public void onRead(UIEdit edit, Element e) throws Exception {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onWrite(UIEdit edit, Element e) throws Exception
	{
		
	}	
	
	@Override
	public void readComplete(UIEdit edit) {}
	@Override
	public void writeBefore(UIEdit edit) {}
}
