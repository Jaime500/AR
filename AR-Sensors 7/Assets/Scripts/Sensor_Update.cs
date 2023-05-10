using UnityEngine;
using TMPro;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System;

public class Sensor_Update : MonoBehaviour
{
    public string deviceId;
    public int deviceClass;

    private TextMeshPro textMesh;
    private float time;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        time = Time.time - 2;

        UpdateSensor();
    }

    public void UpdateSensor()
    {
        Debug.Log("Device ID: " + deviceId);
        if (Time.time > time + 1)
        {
            switch (deviceClass)
            {
                case 0: // Awair
                    update_awair();
                    break;
                case 1: // temp and humidity
                    update_temp_humid();
                    break;
                case 2: // co2
                    update_co2();
                    break;
                case 3: // door
                    update_door();
                    break;
                case 4: // light
                    update_light();
                    break;
                case 5: // dual motion
                    update_dual_motion();
                    break;
                default:
                    break;
            }

            time = Time.time;
        }
    }

    // Get the most recent value from the specified sensor
    async Task<string> get_data(string sensor_type, string device_id, bool location = false)
    {
        const string address = "https://influx.linklab.virginia.edu:443";
        const string username = "ar-capstone";
        const string password = "ohh5JaiRa2eda5iec3tooN0iec7no1Ee";
        const string dbName = "gateway-generic";

        const string authValue = (username + ":" + password);

        AuthenticationHeaderValue auth = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(authValue)));
        HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(address),
            DefaultRequestHeaders = { Authorization = auth }
        };

        string query = "SELECT value" + (location ? ", location_specific" : "") + " FROM \"" + sensor_type + "\" WHERE device_id = '" + device_id + "' ORDER BY time DESC LIMIT 1;";
        HttpResponseMessage response = await httpClient.GetAsync(("/query?db=" + dbName + "&q=" + query).Replace(" ", "%20"));

        string response_string = await response.Content.ReadAsStringAsync();
        string dirty_value = response_string.Substring(response_string.LastIndexOf(',') + 1);
        string value = dirty_value.Remove(dirty_value.IndexOf(']'));
        return value;
    }

    async void update_awair()
    {
        Task<string> location = get_data("Temperature_°C", deviceId, true);
        Task<string> response0 = get_data("Temperature_°C", deviceId);
        Task<string> response1 = get_data("Humidity_%", deviceId);
        Task<string> response2 = get_data("co2_ppm", deviceId);
        Task<string> response3 = get_data("voc_ppb", deviceId);
        Task<string> response4 = get_data("pm2.5_μg/m3", deviceId);
        Task<string> response5 = get_data("awair_score", deviceId);

        await Task.WhenAll(location, response0, response1, response2, response3, response4, response5);

        string locationtext = location.Result;
        string response0text = response0.Result;
        string response1text = response1.Result;
        string response2text = response2.Result;
        string response3text = response3.Result;
        string response4text = response4.Result;
        string response5text = response5.Result;

        locationtext = locationtext.Substring(1, locationtext.Length - 2);
        string response0text2 = (float.Parse(response0text) * 1.8 + 32).ToString();
        if (response0text2.Length > 5)
            response0text2 = response0text2.Substring(0, 5);
        textMesh.text = " AWAIR " + locationtext + "\r\n Temp (°C): " + response0text + "\r\n Temp (°F): " + response0text2 + "\r\n Humidity (%): " + response1text +
            "\r\n CO2 (ppm): " + response2text + "\r\n VOC (ppb): " + response3text + "\r\n pm2.5 (μg/m^3): " + response4text + "\r\n Awair Score: " + response5text;
    }

    async void update_temp_humid()
    {
        Task<string> location = get_data("Temperature_°C", deviceId, true);
        Task<string> response0 = get_data("Temperature_°C", deviceId);
        Task<string> response1 = get_data("Humidity_%", deviceId);
        // Task<string> response2 = get_data("T-Sensor", deviceId);
        // Task<string> response3 = get_data("rssi", deviceId);

        await Task.WhenAll(location, response0, response1);

        string locationtext = location.Result;
        string response0text = response0.Result;
        string response1text = response1.Result;
        //string response2text = response2.Result;
        //string response3text = response3.Result;

        locationtext = locationtext.Substring(1, locationtext.Length - 2);
        string response0text2 = (float.Parse(response0text) * 1.8 + 32).ToString();
        if (response0text.Length > 5)
            response0text = response0text.Substring(0, 5);
        if (response0text2.Length > 5)
            response0text2 = response0text2.Substring(0, 5);
        if (response1text.Length > 4)
            response1text = response1text.Substring(0, 4);
        textMesh.text = " Temperature/Humidity\r\n " + locationtext + "\r\n Temp (°C): " + response0text + "\r\n Temp (°F): " + response0text2 + "\r\n Humidity (%): " + response1text;
        //+ "\r\n T-Sensor: " + response2text + "\r\n RSSI: " + response3text;
    }

    async void update_co2()
    {
        Task<string> location = get_data("Concentration_ppm", deviceId, true);
        Task<string> response0 = get_data("Concentration_ppm", deviceId);
        Task<string> response1 = get_data("Temperature_°C", deviceId);
        Task<string> response2 = get_data("Humidity_%", deviceId);
        // Task<string> response3 = get_data("rssi", deviceId);

        await Task.WhenAll(location, response0, response1, response2);

        string locationtext = location.Result;
        string response0text = response0.Result;
        string response1text = response1.Result;
        string response2text = response2.Result;
        //string response3text = response3.Result;
        
        locationtext = locationtext.Substring(1, locationtext.Length - 2);
        string response1text2 = (float.Parse(response1text) * 1.8 + 32).ToString();
        if (response1text.Length > 5)
            response1text = response1text.Substring(0, 5);
        if (response1text2.Length > 5)
            response1text2 = response1text2.Substring(0, 5);
        if (response2text.Length > 4)
            response2text = response2text.Substring(0, 4);
        textMesh.text = " CO2\r\n " + locationtext + "\r\n CO2 (ppm): " + response0text + "\r\n Temp (°C): " + response1text + "\r\n Temp (°F): " + response1text2 + "\r\n Humidity (%): " + response2text;
        //+ "\r\n RSSI: " + response3text;
    }

    async void update_door()
    {
        Task<string> location = get_data("Contact", deviceId, true);
        Task<string> response0 = get_data("Contact", deviceId);
        //Task<string> response1 = get_data("rssi", deviceId);

        await Task.WhenAll(location, response0);

        string locationtext = location.Result;
        string response0text = response0.Result;
        //string response1text = response1.Result;

        response0text = (float.Parse(response0text) == 0.0) ? "Open" : "Closed";

        locationtext = locationtext.Substring(1, locationtext.Length - 2);
        textMesh.text = " Door\r\n " + locationtext + "\r\n Closed: " + response0text;
        //+"\r\n RSSI " + response1;
    }

    async void update_light()
    {
        Task<string> location = get_data("Illumination_lx", deviceId, true);
        Task<string> response0 = get_data("Illumination_lx", deviceId);
        //Task<string> response1 = get_data("Range select", deviceId);
        Task<string> response2 = get_data("Supply voltage_V", deviceId);
        //Task<string> response3 = get_data("rssi", deviceId);

        await Task.WhenAll(location, response0, response2);

        string locationtext = location.Result;
        string response0text = response0.Result;
        //string response1text = response1.Result;
        string response2text = response2.Result;
        //string response3text = response3.Result;

        locationtext = locationtext.Substring(1, locationtext.Length - 2);
        if (response2text.Length > 4)
            response2text = response2text.Substring(0, 4);
        textMesh.text = " Light\r\n " + locationtext + "\r\n Luxes: " + response0text
            //+ "\r\n Range Sel: " + response1text + 
            + "\r\n Voltage Supply: " + response2text;
        //+ "\r\n RSSI: " + response3text;
    }

    async void update_dual_motion()
    {
        Task<string> location = get_data("PIR Status", deviceId, true);
        Task<string> response0 = get_data("PIR Status", deviceId);
        Task<string> response1 = get_data("Supply voltage (OPTIONAL)_V", deviceId);
        //Task<string> response2 = get_data("Supply voltage availability", deviceId);
        //Task<string> response3 = get_data("rssi", deviceId);

        await Task.WhenAll(location, response0, response1);

        string locationtext = location.Result;
        string response0text = response0.Result;
        string response1text = response1.Result;
        //string response2text = response2.Result;
        //string response3text = response3.Result;

        locationtext = locationtext.Substring(1, locationtext.Length - 2);
        response0text = (Int32.Parse(response0text) < 255) ? "Vacant" : "Occupied";


        textMesh.text = " Dual Motion\r\n " + locationtext + "\r\n PIR Status: " + response0text + "\r\n Voltage Supply: " + response1text;
        // + "\r\n Supply Available?: " + response2text + "\r\n RSSI: " + response3text;
    }
}
