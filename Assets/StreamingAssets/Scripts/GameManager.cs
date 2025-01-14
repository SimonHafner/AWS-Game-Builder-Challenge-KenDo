using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // spriteElements contains all sprite objects representing 
    [SerializeField] public List<Sprite> spriteElements;
    // List of background block elements used for blocked values, ordered after the indexes of spriteElements
    [SerializeField] public List<GameObject> backgroundElements;
    [SerializeField] public Sprite tileSprtieUnselected;
    [SerializeField] public Sprite tileSpriteSelected;

    // folderPath references the path to data folder
    string folderPathData = Application.dataPath + "/KenDo!/Data/1-2-3-L45/";

    private List<List<int>> indexValues;
    public Button selectedButton;


    public GameObject tableObject; // Reference to the Table game object

    public List<List<string>> GetTileSourceImages()
    {
        List<List<string>> sourceImages = new List<List<string>>();

        // Get all the Image components from the Tile game objects
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        Image[] images = new Image[tiles.Length];
        for (int i = 0; i < tiles.Length; i++)
        {
            images[i] = tiles[i].transform.Find("Button/Item").GetComponent<Image>();
        }

        // Determine the dimensions of the 2D list
        int numRows = 0;
        int numCols = 0;
        foreach (Image image in images)
        {
            // Parse the x and y coordinates from the image's name
            string[] nameParts = image.gameObject.name.Split('-');
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);

            // Update the dimensions of the 2D list if necessary
            if (x >= numCols)
            {
                numCols = x + 1;
            }
            if (y >= numRows)
            {
                numRows = y + 1;
            }
        }

        // Initialize the 2D list
        for (int i = 0; i < numRows; i++)
        {
            List<string> row = new List<string>();
            for (int j = 0; j < numCols; j++)
            {
                row.Add("");
            }
            sourceImages.Add(row);
        }

        // Populate the 2D list with the source images from the tiles
        foreach (Image image in images)
        {
            // Parse the x and y coordinates from the image's name
            string[] nameParts = image.gameObject.name.Split('-');
            int x = int.Parse(nameParts[1]);
            int y = int.Parse(nameParts[2]);

            // Get the source image name from the image component
            sourceImages[y][x] = image.sprite.name;
        }

        return sourceImages;
    }

    public void ReplaceItemSourceImage(int x, int y, Sprite sprite)
    {
        // Find the Tile game object with the specified x and y coordinates
        string tileName = string.Format("Tile - {0}-{1}", x, y);
        GameObject tile = GameObject.Find(tileName);

        if (tile != null)
        {
            // Find the Image component of the Item game object
            Image image = tile.transform.Find("Button/Item").GetComponent<Image>();

            // Replace the source image of the Image component with the specified sprite
            if (sprite != null)
            {
                image.sprite = sprite;
            }
        }
    }

    public void ReplaceTileSourceImage(int x, int y, Sprite sprite)
    {
        // Find the Tile game object with the specified x and y coordinates
        string tileName = string.Format("Tile - {0}-{1}", x, y);
        GameObject tile = GameObject.Find(tileName);

        if (tile != null)
        {
            // Find the Image component of the Item game object
            Image image = tile.transform.Find("Button").GetComponent<Image>();

            // Replace the source image of the Image component with the specified sprite
            if (sprite != null)
            {
                image.sprite = sprite;
            }
        }
    }

    public void SwitchSelectionBlock(int x, int y)
    {
        // Find the Tile game object with the specified x and y coordinates
        string tileName = string.Format("Tile - {0}-{1}", x, y);
        GameObject tile = GameObject.Find(tileName);

        if (tile != null)
        {
            // Find the GameObject for the SelectionBlock component
            GameObject selectionBlock = tile.transform.Find("Button/SelectionBlock").gameObject;
            
            // Set the active state of the SelectionBlock component
            if (selectionBlock.activeSelf){
                selectionBlock.SetActive(false);
            }
            else{
                selectionBlock.SetActive(true);
            }
        }
    }

    public void BlockTileButton(int x, int y, int index){
        // Find the Tile game object with the specified x and y coordinates
        string tileName = string.Format("Tile - {0}-{1}", x, y);
        GameObject tile = GameObject.Find(tileName);
        GameObject button = tile.transform.Find("Button").gameObject;
        GameObject blocker = tile.transform.Find("Blocker").gameObject;
        button.GetComponent<Button>().GetBackgroundElement(index).gameObject.SetActive(true);
        blocker.SetActive(true);
    }

    public void UnblockTileButton(int x, int y){
        // Find the Tile game object with the specified x and y coordinates
        string tileName = string.Format("Tile - {0}-{1}", x, y);
        GameObject tile = GameObject.Find(tileName);
        GameObject button = tile.transform.Find("Button").gameObject;
        GameObject blocker = tile.transform.Find("Blocker").gameObject;
        button.GetComponent<Button>().DeactivateAllBackgroundElements();
        blocker.SetActive(false);
    }

    


    // updates all item source images according to the data of indexValues
    // the function set all blockers for the present values accordingly 
    public void UpdateAllItemSourceImages()
    {
    for (int x = 0; x < indexValues.Count; x++)
    {
        for (int y = 0; y < indexValues[x].Count; y++)
        {
            // getting the index value of the data table
            int index = indexValues[x][y];

            // setting the image for the item
            if (index >= 0 && index < spriteElements.Count)
            {
                ReplaceItemSourceImage(x, y, spriteElements[index]);
                if (index > 0)
                {
                    BlockTileButton(x,y,index-1);
                }
                if (index == 0){
                    UnblockTileButton(x,y);
                }
            }
        }
    }
    }
public void OnButtonSelect(Button button)
{
    // Debug.Log(button.GetPosition());

    if (selectedButton != null)
    {
        (int x, int y) position_t0 = selectedButton.GetPosition();
        SwitchSelectionBlock(position_t0.x, position_t0.y);
    }
    if (selectedButton == button){
        selectedButton = null;
    }
    else{
        selectedButton = button;
        (int x, int y) position_t1 = button.GetPosition();
        SwitchSelectionBlock(position_t1.x, position_t1.y); 
    }
    
    }
    public void OnAssignmentButtonClick(int i){
        if (selectedButton != null){
            (int x, int y) position = selectedButton.GetPosition();
            SwitchSelectionBlock(position.x, position.y);
            ReplaceItemSourceImage(position.x, position.y, spriteElements[i]);
            indexValues[position.x][position.y] = i;
            selectedButton = null;
        }
    }

    public void ResetGame(){
        this.UpdateAllItemSourceImages();
    }

    public void LoadData(){
        List<string> txtFileNames = TextFileParser.GetTxtFilesInFolder(folderPathData);
        int fileIndex = RandomHelper.Range(0, txtFileNames.Count);
        indexValues = TextFileParser.ParseFile(folderPathData +txtFileNames[fileIndex]);
    }
    private void Awake() {
        LoadData();
        ResetGame();
    }



    // Start is called before the first frame update
    private void Start()
    {

    }
}