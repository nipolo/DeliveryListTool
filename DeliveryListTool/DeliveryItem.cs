using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryListTool
{
    public class DeliveryItem
    {
        string _itemCode;
        int _quantity;
        string _batchNumber;
        DateTime _expirationDate;
        string _daliveryName;
        bool _isReservedForClient;
        string _currencyInitials;

        public bool IsReservedForClient => _isReservedForClient;

        public DeliveryItem(string itemCode, int quantity, string batchNumber, 
            DateTime expirationDate, string daliveryName, string currencyInitials)
        {
            _itemCode = itemCode;
            _quantity = quantity;
            _batchNumber = batchNumber;
            _expirationDate = expirationDate;
            _daliveryName = daliveryName;
            _currencyInitials = currencyInitials;
            _isReservedForClient = false;
        }

        public bool CheckForReservation (string itemCode, int quantity)
        {
            if(_itemCode == itemCode && _quantity == quantity && !_isReservedForClient)
            {                
                return _isReservedForClient = true;
            }

            return false;
        }

        public override string ToString()
        {
            return _itemCode + ","
                + _daliveryName + ","
                + _batchNumber + ","
                + _expirationDate.ToString(CultureInfo.CreateSpecificCulture("bg-BG")) + ","
                + _quantity + ","
                + "," // Empty column for Zeron
                + "," // Empty column for unit price
                + _currencyInitials;
        }
    }
}
