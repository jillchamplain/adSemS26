using UnityEngine;

public enum BlockShape
{
    VERTICAL,
    HORIZONTAL,
    NUM_SHAPES
}
public class Block : MonoBehaviour
{
    [SerializeField] BlockShape shape;
    public BlockShape getShape() {  return shape; }
    [SerializeField] MatchItemType type;
    public MatchItemType getMatchItemType() { return type; }
    public void setMatchItemType(MatchItemType newType) {  type = newType; }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
