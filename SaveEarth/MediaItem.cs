using System;
using SQLite;
using Xamarin.Forms;

namespace SaveEarth
{
    public class MediaItemToUpload
    {
       
            [PrimaryKey, AutoIncrement]
            public int primaryKey { get; set; }

            public byte[] itemData { get; set; }

            public string mediaLocation { get; set; }
        
    }
}
