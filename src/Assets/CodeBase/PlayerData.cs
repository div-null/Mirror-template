using Mirror;

namespace Game.CodeBase
{
    public enum ColorType
    {
        Red = 0,
        Green = 1,
        Blue,
        Yellow,
        Purple
    }

    public class PlayerData : NetworkBehaviour
    {
        public string Username;
        //public ColorType ColorType;
        //public int SkinId;
        //
        //public override void OnStartAuthority()
        //{
        //    //get input manager from service locator
        //    //connect actions to methods
        //}
        //
        //[Client]
        //public void Move(Vector2 direction)
        //{
        //    
        //}
        //
        //// Start is called before the first frame update
        //void Start()
        //{
        //    
        //}
        //
        //// Update is called once per frame
        //void Update()
        //{
        //    
        //}
    }
}