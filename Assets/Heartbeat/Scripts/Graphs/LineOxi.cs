// Copyright (c) 2014, Tokyo University of Science All rights reserved.
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. * Neither the name of the Tokyo Univerity of Science nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class LineOxi : MonoBehaviour {
	public HeartBeatOxi oxi;
    public double divisor = 3000;
	public double offset = 1.0;
    private LineRenderer line;

	public int type = 0;

	// Use this for initialization
	void Start () {
		oxi = GameObject.Find ("CompositionEngine").GetComponent<HeartBeatOxi>();
        line = (LineRenderer) this.GetComponent("LineRenderer");
		line.SetVertexCount(oxi.pulseHistorySize);
	}
	
	/// <summary>
	/// Draw the new point of the line
	/// </summary>
	void Update () {

        int i = 0;
		Queue<HeartBeatOxi.PulseData> buffer = oxi.getBuffer();
		lock(oxi.sync_data){
			foreach(HeartBeatOxi.PulseData data in buffer)
    	    {

				float posX = (float) (-7.5f+15f*((1.0/oxi.pulseHistorySize)*i));
				float value = 0f;

				if(type == 0){
					value = data.raw;
				}else if(type == 1){
					value = data.beat_raw;
				}else if(type == 2){
					value = data.beat_stauts;
				}
				float posY = (float) (value / divisor - offset);
            	line.SetPosition(i, new Vector3(posX, posY, 0));
            	i++;
			}	
		}
       
	}
}
