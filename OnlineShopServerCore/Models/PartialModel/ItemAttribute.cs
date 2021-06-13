using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShopServerCore.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopServerCore.Models
{
    public partial class ItemAttribute
    {
        public string PatchFromRequest(JSONItemAttribute attr, ref OnlineShopContext _context)
        {
            Item databaseItem = _context.Find<Item>(attr.itemId);
            if (databaseItem == null)
            {
                return "Не найден товар";
            }

            CategoryAttribute databaseCategoryAttribute = null;
            if (attr.categoryAttribute != null)
            {
                databaseCategoryAttribute = _context.CategoryAttributes
                    .Include(a=>a.Type)
                    .Where(a=> a.Id == attr.categoryAttribute.id).FirstOrDefault();
                if (databaseCategoryAttribute == null) return "Не найден атрибут категории";
            }
            if (attr.ItemAttributesValues == null || attr.ItemAttributesValues.Count == 0)
            {
                return "Не задано значений для атрибута";
            }
           bool isAvailableAttr = _context.CategoryAttributes.Where(ca => databaseItem.CategoryId == ca.CategoryId
            && databaseCategoryAttribute.Id == ca.Id).Count() > 0;
            if (!isAvailableAttr)
            {
                return "Данный атрибут категории недоступен для текущего товара";
            }
            this.ItemId = databaseItem.Id;
            this.CategoryAttributesId =databaseCategoryAttribute.Id;

            //Добавление новых значений для атрибута товара
            this.ItemAttributesValues = new List<ItemAttributesValue>();
            foreach (var value in attr.ItemAttributesValues)
            {
                ItemAttributesValue v = new ItemAttributesValue();
                v.ItemAttribute = this;
                
                if (value.Unit != null)
                {
                   Unit u = _context.Find<Unit>(value.Unit.id);
                    v.Unit = u;
                }
                string type = databaseCategoryAttribute.Type.Name;
                object obj = HelperUtils.TryParseValue<object>(type, value.Value, ref _context);
                if (obj != null)
                {
                    v.Value = value.Value;
                }
                else
                {
                    return $"Некорректное значение \"{value.Value}\" поля типа {type}";
                }
                this.ItemAttributesValues.Add(v);
            }
            return null;
        }
    }
}
