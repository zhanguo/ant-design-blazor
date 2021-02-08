using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace AntDesign
{
    public partial class TansferList
    {
        [CascadingParameter]
        private Transfer Transfer { get; set; }

        [Parameter]
        public TransferDirection Direction { get; set; }

        [Parameter]
        public string TitleText { get; set; }

        [Parameter]
        public bool ShowSelectAll { get; set; }

        [Parameter]
        public string ItemUnit { get; set; }

        [Parameter]
        public string ItemsUnit { get; set; }

        [Parameter]
        public string Filter { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool ShowSearch { get; set; }

        [Parameter]
        public string SearchPlaceholder { get; set; }

        [Parameter]
        public string NotFoundContent { get; set; }

        /// <summary>
        /// (inputValue: string, item: TransferItem) => boolean
        /// </summary>
        [Parameter]
        public Func<string, TransferItem, bool> FilterOption { get; set; }

        [Parameter]
        public IEnumerable<TransferItem> DataSource { get; set; }

        [Parameter]
        public RenderFragment RenderList { get; set; }

        [Parameter]
        public RenderFragment Render { get; set; }

        [Parameter]
        public string Footer { get; set; } = string.Empty;

        [Parameter]
        public RenderFragment FooterTemplate { get; set; }

        [Parameter]
        public EventCallback<bool> HandleSelectAll { get; set; }

        [Parameter]
        public EventCallback<TransferItem> HandleSelect { get; set; }

        [Parameter]
        public EventCallback<(string direction, string value)> FilterChange { get; set; }

        private string _filterValue;

        private record Stat(bool CheckAll, bool CheckHalf, int CheckCount, int ShownCount);

        private Stat _stat = new(false, false, 0, 0);

        private async Task HandleSearch(ChangeEventArgs e, string direction, bool mathTileCount = true)
        {
            _filterValue = e.Value.ToString();
            DataSource = DataSource.Where(a => !_targetKeys.Contains(a.Key) && a.Title.Contains(_leftFilterValue, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (mathTileCount)
                MathTitleCount();

            //if (OnSearch.HasDelegate)
            //{
            //    await OnSearch.InvokeAsync(new TransferSearchArgs(direction, e.Value.ToString()));
            //}
        }

        private async Task ClearFilterValueAsync(string direction)
        {
            if (direction == TransferDirection.Left)
            {
                _leftFilterValue = string.Empty;
                await HandleSearch(new ChangeEventArgs() { Value = string.Empty }, direction);
            }
            else
            {
                _rightFilterValue = string.Empty;
                await HandleSearch(new ChangeEventArgs() { Value = string.Empty }, direction);
            }
        }

        private void MathTitleCount()
        {
            _rightButtonDisabled = _sourceSelectedKeys.Count == 0;
            _leftButtonDisabled = _targetSelectedKeys.Count == 0;

            var leftSuffix = _leftDataSource.Count() == 1 ? Locale.ItemUnit : Locale.ItemsUnit;
            var rightSuffix = _rightDataSource.Count() == 1 ? Locale.ItemUnit : Locale.ItemsUnit;

            var leftCount = _sourceSelectedKeys.Count == 0 ? $"{_leftDataSource.Count()}" : $"{_sourceSelectedKeys.Count}/{_leftDataSource.Count()}";
            var rightCount = _targetSelectedKeys.Count == 0 ? $"{_rightDataSource.Count()}" : $"{_targetSelectedKeys.Count}/{_rightDataSource.Count()}";

            _leftCountText = $"{leftCount} {leftSuffix}";
            _rightCountText = $"{rightCount} {rightSuffix}";

            CheckAllState();
        }
    }
}
