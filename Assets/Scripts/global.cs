using System.Collections.Generic;
public class APIForm
{
    public string name;
    public string token;
    public float betAmount;
    public float cashoutAt = 0;
}
public class Users
{
    public string name;
    public string amount;
}
public class Betted_User_Info
{
    public List<Users> _user;

}
