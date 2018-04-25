package com.example.myproject;

import java.io.DataInputStream;
import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.URL;
import java.net.URLConnection;
import java.util.UUID;

import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.app.Activity;

import android.app.PendingIntent;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.telephony.SmsManager;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

public class Main extends Activity implements OnClickListener
{
	//GUI elements---------------------------------------------------------------------------------
	private EditText txtVehicleID;
	private EditText txtSlotID;
	
	private Button btnGetStatus;
	private Button btnReserve;
	
	private TextView lblStatus;
	
	private TextView lblResponse;
	
	// HTTP-Global Variables-------------------------------------------------------------------------
	HTTPReceive trdHTTPReceive = new HTTPReceive();
	private Handler HTTPhandler = new Handler();

	//Application Variables------------------------------------------------------------------------
    
    
	@Override
    protected void onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
        //Linking the GUI elements
        txtVehicleID = (EditText)findViewById(R.id.txtVehicleID);
        txtSlotID = (EditText)findViewById(R.id.txtSlotID);
        
        btnReserve = (Button)findViewById(R.id.btnReserve); btnReserve.setOnClickListener(this);
        btnGetStatus = (Button)findViewById(R.id.btnGetStatus); btnGetStatus.setOnClickListener(this);
        
        lblStatus = (TextView)findViewById(R.id.lblStatus);
        
        lblResponse = (TextView)findViewById(R.id.lblResponse);
        
        trdHTTPReceive.start();
        
    }
	//User Interaction#############################################################################
	@Override //Capture the clicking event of various assigned objects - views 
	public void onClick(View src) 
	{
		switch(src.getId())
		{
			case R.id.btnGetStatus:
				new HTTPPOST().execute("LIG_2111718.txt","MOBILESTATUS*");
			break;
		
			case R.id.btnReserve:
				new HTTPPOST().execute("LIG_2111718.txt","MOBILERESERVE*" + txtVehicleID.getText().toString() +  "*" + txtSlotID.getText().toString());
			break;
		
			
		}
		
	}
	//#############################################################################################
	//HTTP Client//////////////////////////////////////////////////////////////////////////////////
    /*
    	//ADD THE FOLLOWING IN THE MANIFEST.xml FILE
  		<uses-permission android:name="android.permission.INTERNET"></uses-permission>
  		
  		//HTTP Variables--------------------------------------------------------------------------------

		//to use with thread only
		HTTPReceive trdHTTPReceive = new HTTPReceive();
		private Handler HTTPhandler = new Handler();
   		
    */
	//--------------------------------------------------------------------------
	private String UploadPOST(String LIG_XXXXXXX, String Data)
    {
        StringBuffer sbResponse = new StringBuffer();
        String TxData = LIG_XXXXXXX + Data;
        try
        {
        	URL url = new URL("http://www.iot.logicinside.net/");
            URLConnection connection = url.openConnection();
        	
            // We need to make sure we specify that we want to provide input and
            // get output from this connection. We also want to disable caching,
            // so that we get the most up-to-date result. And, we need to
            // specify the correct content type for our data.
            connection.setDoInput(true);
            connection.setDoOutput(true);
            connection.setUseCaches(false);
            connection.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
        	
            // Send the POST data
            DataOutputStream dataOut = new DataOutputStream(connection.getOutputStream());
            dataOut.writeBytes(TxData);
            dataOut.flush();
            dataOut.close();
        	
 	        // get the response from the server and store it in result
            DataInputStream dataIn = new DataInputStream(connection.getInputStream());
            int ch; while((ch=dataIn.read())!=-1) sbResponse.append( (char) ch );
            dataIn.close();
            
        } catch (IOException e) 
        {
            e.printStackTrace();
            return "NO GPRS CONNECTION.";
        }
        return sbResponse.toString();    
    }
	//--------------------------------------------------------------------------
	//to call----new HTTPPOST().execute("LIG_7051314.txt",TxPacket);
	private class HTTPPOST extends AsyncTask<String, Integer, Double>
	{
		@Override
		protected Double doInBackground(String... params) 
		{
			UploadPOST(params[0],params[1]);
			return null;
		}
	}
    //--------------------------------------------------------------------------
	private String Download(String LIG_XXXXXXX)
    {
    	StringBuffer sbResponse = new StringBuffer();
        try
        {
        	URL url = new URL("http://www.iot.logicinside.net/" + LIG_XXXXXXX);
            URLConnection connection = url.openConnection();
        	
            // get the response from the server and store it in result
            DataInputStream dataIn = new DataInputStream(connection.getInputStream());
            int ch; while((ch=dataIn.read())!=-1) sbResponse.append( (char) ch );
            dataIn.close();
            
        } catch (IOException e) 
        {
            e.printStackTrace();
            return "NO GPRS CONNECTION.";
        }
        return sbResponse.toString(); 
    }
    //--------------------------------------------------------------------------
	public class HTTPReceive extends Thread  
    {
		boolean fStop=false;
		String RxHTTP,RxData;
		
		@Override
		public void run() 
		{
    		while(!fStop)
			{
    			try {Thread.sleep(1000);}catch(InterruptedException e){}	//set thread sleep time---important
    			//fetch HTTP Server Data---------------------------------------
    			RxHTTP=Download("LIG_2111718.txt");
    			if(RxHTTP.startsWith("SERVER"))
                {
                    RxData=RxHTTP.substring(6);
                }
                else
                {
                    continue;
                }
    			//data received ---Process ON DATA-----------------------------
    			
    			if(RxData.toCharArray()[0] =='S')//status
    			{
					HTTPhandler.post(new Runnable() 
					{
	                    public void run() 
	                    {
	                    	lblStatus.setText("S1:" + RxData.toCharArray()[1]+ " S2:" + RxData.toCharArray()[2] + " S3:" + RxData.toCharArray()[3]+" S4:" + RxData.toCharArray()[4]);
	                    }
					});
    			}
   			
    			if(RxData.toCharArray()[0] =='M')//Msg
    			{
					HTTPhandler.post(new Runnable() 
					{
	                    public void run() 
	                    {
	                    	lblResponse.setText(RxData.substring(1));
	                    	Toast.makeText(getBaseContext(),RxData.substring(1),Toast.LENGTH_LONG).show();
	                    }
					});
    			}
				
				UploadPOST("LIG_2111718.txt","NOTHING");
				
			}
		}
    }
    //HTTP Client End \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

}
