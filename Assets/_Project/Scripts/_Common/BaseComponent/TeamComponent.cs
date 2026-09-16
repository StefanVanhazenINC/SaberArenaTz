namespace Common.BaseComponent
{
    using UnityEngine;

    public enum TypeTeam 
    {
        Any = 0,
        Player = 1,
        Enemy= 2 ,
        DestuctableTeam = 3,
    }
    public class TeamComponent : MonoBehaviour
    {
        [SerializeField] private TypeTeam _teamType;

        public TypeTeam TeamType { get => _teamType; }
        public bool Self(TeamComponent team) 
        {
            if (team == this)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        public bool CheckWrongTeam(TypeTeam typeTeam)
        {
            if (_teamType== TypeTeam.Any) 
            {
                return true;
            }
            if (_teamType == typeTeam)
            {
                return false;
            }
            else if (_teamType != typeTeam)
            {
                return true;
            }
            else 
            {
                return true;
            }
            
        }
    }
}
