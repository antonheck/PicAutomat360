using System;

public class ViewItemDb
{
    private string _name;
    private string _timeStamp;
    private int _Id;

    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    public string TimeStamp
    {
        get
        {
            return _timeStamp;
        }

        set
        {
            _timeStamp = value;
        }
    }

    public int Id
    {
        get
        {
            return _Id;
        }

        set
        {
            _Id = value;
        }
    }
}
