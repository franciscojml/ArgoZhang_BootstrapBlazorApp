using BootstrapBlazor.Components;
using BootstrapBlazorApp2.Server.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BootstrapBlazorApp2.Server.Components.Pages
{
    
    /// <summary>
    /// 表格编辑示例
    /// </summary>
    public partial class TableDemo : ComponentBase
    {
        [Inject]
        [NotNull]
        private IStringLocalizer<Foo>? Localizer { get; set; }

        private readonly ConcurrentDictionary<Foo, IEnumerable<SelectedItem>> _cache = new();

        private IEnumerable<SelectedItem> GetHobbies(Foo item) => _cache.GetOrAdd(item, f => Foo.GenerateHobbies(Localizer));

        private static IEnumerable<int> PageItemsSource => new int[] { 20, 40 };

        private readonly int[] PageItemsSourceFoo = [5, 10, 15, 20];

        [NotNull]
        private List<Foo>? Items { get; set; }

        /// <summary>
        /// 查询操作方法
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private Task<QueryData<Foo>> OnQueryAsync(QueryPageOptions options)
        {
            // 此处代码实战中不可用，仅仅为演示而写防止数据全部被删除
            if (Items == null || Items.Count == 0)
            {
                Items = Foo.GenerateFoo(Localizer);
            }

            var items = Items.Where(options.ToFilterFunc<Foo>());
            // 排序
            var isSorted = false;
            if (!string.IsNullOrEmpty(options.SortName))
            {
                items = items.Sort(options.SortName, options.SortOrder);
                isSorted = true;
            }

            var total = items.Count();
            return Task.FromResult(new QueryData<Foo>()
            {
                Items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList(),
                TotalCount = total,
                IsFiltered = true,
                IsSorted = isSorted,
                IsSearch = true
            });
        }

        private Task<bool> OnSaveAsync(Foo foo, ItemChangedType changedType)
        {
            var ret = false;
            if (changedType == ItemChangedType.Add)
            {
                var id = Items.Count + 1;
                while (Items.Find(item => item.Id == id) != null)
                {
                    id++;
                }
                var item = new Foo()
                {
                    Id = id,
                    Name = foo.Name,
                    Address = foo.Address,
                    Complete = foo.Complete,
                    Count = foo.Count,
                    DateTime = foo.DateTime,
                    Education = foo.Education,
                    Hobby = foo.Hobby
                };
                Items.Add(item);
            }
            else
            {
                var f = Items.Find(i => i.Id == foo.Id);
                if (f != null)
                {
                    f.Name = foo.Name;
                    f.Address = foo.Address;
                    f.Complete = foo.Complete;
                    f.Count = foo.Count;
                    f.DateTime = foo.DateTime;
                    f.Education = foo.Education;
                    f.Hobby = foo.Hobby;
                }
            }
            ret = true;
            return Task.FromResult(ret);
        }

        private Task<bool> OnDeleteAsync(IEnumerable<Foo> foos)
        {
            foreach (var foo in foos)
            {
                Items.Remove(foo);
            }

            return Task.FromResult(true);
        }

        private IEnumerable<Foo> _filterItemsFoo = default!;

        private static string? GetTextCallback(Foo foo) => foo.Name;

        private async Task<QueryData<Foo>> OnFilterQueryAsync(QueryPageOptions options)
        {
            if (_filterItemsFoo == null || _filterItemsFoo.Count() == 0)
            {
                _filterItemsFoo =  Items;
            }

            var items = _filterItemsFoo.Where(options.ToFilterFunc<Foo>());

            if (!string.IsNullOrEmpty(options.SortName))
            {
                items = items.Sort(options.SortName, options.SortOrder);
            }

            var count = items.Count();
            if (options.IsPage)
            {
                items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems);
            }

            return await Task.FromResult(new QueryData<Foo>()
            {
                Items = items.ToList(),
                TotalCount = count,
                IsAdvanceSearch = true,
                IsFiltered = true,
                IsSearch = true,
                IsSorted = true
            });
        }

        private List<CascaderItem> _items = [];
        private string Value { get; set; } = "";

        protected override void OnInitialized()
        {
            Value = "";

            _items =
                    [
                    new CascaderItem("Melbourne", "Melbourne"),
                    new CascaderItem("Sydney", "Sydney"),
                    new CascaderItem("Brisbane", "Brisbane"),
                    ];

                    _items[0].AddItem(new CascaderItem("item1_child1", "Brunswick"));
                    _items[0].AddItem(new CascaderItem("item1_child2", "Fitzroy"));
                    _items[0].AddItem(new CascaderItem("item1_child3", "Carlton"));
                    _items[0].AddItem(new CascaderItem("item1_child4", "Thornbury"));

                    _items[0].Items.ElementAt(0).AddItem(new CascaderItem("item1_child1_child", "so-and-so street"));

                    _items[1].AddItem(new CascaderItem("item2_child1", "Millsons Point"));
                    _items[1].AddItem(new CascaderItem("item2_child2", "Potts Point"));
                    _items[1].AddItem(new CascaderItem("item2_child3", "North Sydney"));

                    _items[2].AddItem(new CascaderItem("item3_child1", "Brisbane"));
                    _items[2].AddItem(new CascaderItem("item3_child2", "Gold Cost"));
        }
    }
}
