using UnityEngine;

namespace Runtime.UI
{
    public class GetBodySegmentation : MonoBehaviour
    {
        private Vector2 truncateduvCoords;
        private Color col;
        private string segment;
        [SerializeField] private Texture2D segmentsHead;
        [SerializeField] private Texture2D segmentsTorso;
        [SerializeField] private Texture2D segmentsArms;
        [SerializeField] private Texture2D segmentsLegs;
        [SerializeField] private Texture2D segmentsNails;
    
        public string GetDescriptor(Vector2 textureCoord)
        {
            truncateduvCoords = new Vector2(Mathf.FloorToInt(textureCoord.x), Mathf.FloorToInt(textureCoord.y));
            string segment = "";
            switch(truncateduvCoords.x)
            {
                case 0:
                    segment =  Head(textureCoord);
                    break;
                case 1:
                    segment =  Torso(textureCoord);
                    break;
                case 2:
                    segment =  Arms(textureCoord);
                    break;
                case 3:
                    segment =  Legs(textureCoord);
                    break;
                case 4:
                    segment =  Nails(textureCoord);
                    break;
                case 6:
                    segment =  Genitals();
                    break;
                default:
                    Debug.LogWarning("The selected UV coordinates " + textureCoord + " are out of bounds.");
                    segment = "Unbekannte Position";
                    break;
            }
        
            string side = GetSide(textureCoord.x - truncateduvCoords.x);

            return $"{segment}, {side}";
        }

        private string Head(Vector2 textureCoord)
        {
            col = segmentsHead.GetPixel(Mathf.FloorToInt((textureCoord.x - truncateduvCoords.x) * segmentsHead.width), Mathf.FloorToInt((textureCoord.y - truncateduvCoords.y) * segmentsHead.height));
            switch (Mathf.Round(col.r * 10.0f) * 0.1f, Mathf.Round(col.g * 10.0f) * 0.1f, Mathf.Round(col.b * 10.0f) * 0.1f) //segmentation mask colors are rounded to 1 decimal point so that small differences in color values do not affect final result
            {
                case (0f,0f,0f): //black
                    return("Mittelgesicht");
                case (1f,0f,0f): //red
                    return("Ohr");
                
                case (0f,1f,0f): //green
                    return("Hals");
                
                case (0f,0f,1f): //blue
                    return("Nacken");
                
                case (1f,1f,0f): //yellow
                    return("Hinterkopf");
                
                case (1f,0f,1f): //pink
                    return("Kopfhaut");

                case(0.5f,0f,0f): //Dark Red
                    return("Wange");
                
                case(0f,0.5f,0f): //Dark Green
                    return("Stirn"); 
                
                default:
                    return DefaultCase("Kopf", textureCoord);
                                                                               
            }
        
        }
        private string Torso(Vector2 textureCoord)
        {
            col = segmentsTorso.GetPixel(Mathf.FloorToInt((textureCoord.x - truncateduvCoords.x) * segmentsHead.width), Mathf.FloorToInt((textureCoord.y - truncateduvCoords.y) * segmentsTorso.height));
            switch ((col.r,col.g,col.b)) 
            {
                case (0f,0f,0f):
                    return("Thorax ventral");
                
                case (1f,0f,0f):
                    return("Thorax dorsal");
                
                case (0f,1f,0f):
                    return("Abdomen");
                
                case (0f,0f,1f):
                    return("Rücken");
                
                case (1f,1f,0f):
                    return("Pelvis ventral");
                
                case (1f,0f,1f):
                    return("Pelvis dorsal");
                
                case (0f,1f,1f):
                    return("Schulter");
                
                case (1f,1f,1f):
                    return("Gluteal");
                    
                default:
                    return DefaultCase("Torso", textureCoord);
                                                                            
            }
        }
        private string Arms(Vector2 textureCoord)
        {
            col = segmentsArms.GetPixel(Mathf.FloorToInt((textureCoord.x - truncateduvCoords.x) * segmentsHead.width), Mathf.FloorToInt((textureCoord.y - truncateduvCoords.y) * segmentsArms.height));
            switch (Mathf.Round(col.r * 10.0f) * 0.1f, Mathf.Round(col.g * 10.0f) * 0.1f, Mathf.Round(col.b * 10.0f) * 0.1f) //segmentation mask colors are rounded to 1 decimal point so that small differences in color values do not affect final result
            {
                case (0f,0f,0f):
                    return("Schulter");
                
                case (1f,0f,0f):
                    return("Oberarm");
                
                case (0f,1f,0f):
                    return("Unterarm");
                
                case (0f,0f,1f):
                    return("Handrücken");
                
                case (1f,1f,0f):
                    return("Handfläche");
                
                case (1f,0f,1f):
                    return("Armbeuge");
                
                case (0f,1f,1f):
                    return("Achsel");
                
                case (1f,1f,1f):
                    return("Daumen");
                     
                case(0.5f,0f,0f): //Dark Red
                    return("Zeigefinger");
                
                case(0f,0.5f,0f): //Dark Green
                    return("Mittelfinger"); 
                
                case(0f,0f,0.5f): //Dark Blue
                    return("Ringfinger");
                
                case(0.5f,0.5f,0f): //Muddy Green
                    return("Kleiner Finger");
                
                default:
                    return DefaultCase("Arm", textureCoord);
                
            }
        }
        private string Legs(Vector2 textureCoord)
        {
            col = segmentsLegs.GetPixel(Mathf.FloorToInt((textureCoord.x - truncateduvCoords.x) * segmentsHead.width), Mathf.FloorToInt((textureCoord.y - truncateduvCoords.y) * segmentsLegs.height));
            switch (Mathf.Round(col.r * 10.0f) * 0.1f, Mathf.Round(col.g * 10.0f) * 0.1f, Mathf.Round(col.b * 10.0f) * 0.1f) //segmentation mask colors are rounded to 1 decimal point so that small differences in color values do not affect final result
            {
                case (0f,0f,0f):
                    return("Oberschenkel ventral");
                
                case (1f,0f,0f):
                    return("Oberschenkel dorsal");
                
                case (0f,1f,0f):
                    return("Knie");
                
                case (0f,0f,1f):
                    return("Kniekehle");
                
                case (1f,1f,0f):
                    return("Unterschenkel ventral");
                
                case (1f,0f,1f):
                    return("Unterschenkel dorsal");
                
                case (0f,1f,1f):
                    return("Fußrücken");
                
                case (1f,1f,1f):
                    return("Fußsohle");
                
                case (0.5f,0f,0f):
                    return("Zehen");

                case(0f,0.5f,0f): //Dark Green
                    return("Fuß medial"); 
                
                case(0f,0f,0.5f): //Dark Blue
                    return("Fuß lateral");

                default:
                    return DefaultCase("Bein", textureCoord);
                                                                                
            }
        }

        private string Nails(Vector2 textureCoord)
        {
            col = segmentsNails.GetPixel(Mathf.FloorToInt((textureCoord.x - truncateduvCoords.x) * segmentsHead.width), Mathf.FloorToInt((textureCoord.y - truncateduvCoords.y) * segmentsNails.height));
            switch ((col.r,col.g,col.b)) 
            {
                case (0f,0f,0f):
                    return("Fingernagel");
            
                case (1f,1f,1f):
                    return("Fußnagel");
        
                default:
                    return DefaultCase("Nagel", textureCoord);     
            }                                       
        }

        private string Genitals()
        {
            return "Genital";      
        }

        private string DefaultCase(string txt, Vector2 textureCoordinates)
        {
            Debug.LogWarning($"The pixel color {col} or rounded pixel color {(Mathf.Round(col.r * 10.0f) * 0.1f, Mathf.Round(col.g * 10.0f) * 0.1f, Mathf.Round(col.b * 10.0f) * 0.1f)} at {textureCoordinates} does not match any predefined segments.");
            return txt;
        }

        private string GetSide(float xCoord)
        {
            if(xCoord > 0.5f)
            {
                return "links";
            }

            return "rechts";
        }
    }
}