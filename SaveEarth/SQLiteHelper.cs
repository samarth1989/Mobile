using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace SaveEarth
{
    public class SQLiteHelper
    {
        SQLiteAsyncConnection db;
        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<MediaItemToUpload>().Wait();
        }

        //Insert and Update new record  
        public Task<int> SaveItemAsync(MediaItemToUpload item)
        {
            if (item.itemData != null)
            {
                return db.UpdateAsync(item);
            }
            else
            {
                return db.InsertAsync(item);
            }
        }

        //Delete  
        public Task<int> DeleteItemAsync(MediaItemToUpload item)
        {
            return db.DeleteAsync(item);
        }

        //Read All Items  
        public Task<List<MediaItemToUpload>> GetItemsAsync()
        {
            return db.Table<MediaItemToUpload>().ToListAsync();
        }


        //Read Item  
        public Task<MediaItemToUpload> GetItemAsync(int itemId)
        {
            return db.Table<MediaItemToUpload>().Where(i => i.primaryKey == itemId).FirstOrDefaultAsync();
        }
    }
}
