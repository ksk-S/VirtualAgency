// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;

public class GUI_Selector : MonoBehaviour {
    private bool PULSE = true;
    public LineRenderer LinePULSE;

	private bool BEAT_RAW = true;
	public LineRenderer LineBEAT_RAW;

	private bool BEAT_STATUS = true;
	public LineRenderer LineBEAT_STATUS;

    private bool ECG = true;
    public LineRenderer LineECG;
  
	// Use this for initialization
	void Start () {
	}
	
	/// <summary>
	/// Update the state of the line renderer
	/// </summary>
	void Update () {
		LinePULSE.enabled = PULSE;
     
        LineECG.enabled = ECG;

		LineBEAT_RAW.enabled = BEAT_RAW;
		
		LineBEAT_STATUS.enabled = BEAT_STATUS;
    
	}

    /// <summary>
    /// Drawn the GUI
    /// </summary>
    void OnGUI () {
        GUI.Box(new Rect(10, 10, 120, 120), "Graphe Selector");
		ECG = GUI.Toggle(new Rect(15, 30, 50, 20), ECG, new GUIContent("ECG"));
		PULSE = GUI.Toggle(new Rect(15, 50, 50, 20), PULSE, new GUIContent("PULSE"));
		BEAT_RAW = GUI.Toggle(new Rect(15, 70, 50, 20), BEAT_RAW, new GUIContent("B-Raw"));
		BEAT_STATUS = GUI.Toggle(new Rect(15, 90, 50, 20), BEAT_STATUS, new GUIContent("B-Status"));

    }
}
