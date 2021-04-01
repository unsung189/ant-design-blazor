﻿using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using OneOf;

namespace AntDesign
{
    using StringNumber = OneOf<string, int>;

    public class EmbeddedProperty
    {
        public StringNumber Span { get; set; }

        public StringNumber Pull { get; set; }

        public StringNumber Push { get; set; }

        public StringNumber Offset { get; set; }

        public StringNumber Order { get; set; }
    }

    public partial class Col : AntDomComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public StringNumber Flex { get; set; }

        [Parameter]
        public StringNumber Span { get; set; }

        [Parameter]
        public StringNumber Order { get; set; }

        [Parameter]
        public StringNumber Offset { get; set; }

        [Parameter]
        public StringNumber Push { get; set; }

        [Parameter]
        public StringNumber Pull { get; set; }

        [Parameter]
        public OneOf<int, EmbeddedProperty> Xs { get; set; }

        [Parameter]
        public OneOf<int, EmbeddedProperty> Sm { get; set; }

        [Parameter]
        public OneOf<int, EmbeddedProperty> Md { get; set; }

        [Parameter]
        public OneOf<int, EmbeddedProperty> Lg { get; set; }

        [Parameter]
        public OneOf<int, EmbeddedProperty> Xl { get; set; }

        [Parameter]
        public OneOf<int, EmbeddedProperty> Xxl { get; set; }
        [Parameter]
        public AntLableAlignType LableAlign { get; set; } = AntLableAlignType.Right;

        [CascadingParameter]
        public Row Row { get; set; }

        private string FlexStyle { get; set; }

        private string GutterStyle { get; set; }

        internal void RowGutterChanged((int horizontalGutter, int verticalGutter) gutter)
        {
            GutterStyle = "";
            if (gutter.horizontalGutter > 0)
            {
                GutterStyle = $"padding-left: {gutter.horizontalGutter / 2}px; padding-right: {gutter.horizontalGutter / 2}px;";
            }
        }

        private void SetHostClassMap()
        {
            var prefixCls = "ant-col";
            this.ClassMapper.Clear()
                .Add(prefixCls)
                .GetIf(() => $"{prefixCls}-{this.Span.Value}", () => this.Span.Value != null)
                .GetIf(() => $"{prefixCls}-order-{this.Order.Value}", () => this.Order.Value != null)
                .GetIf(() => $"{prefixCls}-offset-{this.Offset.Value}", () => this.Offset.Value != null)
                .GetIf(() => $"{prefixCls}-pull-{this.Pull.Value}", () => this.Pull.Value != null)
                .GetIf(() => $"{prefixCls}-push-{this.Push.Value}", () => this.Push.Value != null)
                .If($"{prefixCls}-rtl", () => RTL)
                .If($"ant-form-item-label-left", () => this.LableAlign == AntLableAlignType.Left)
                ;

            SetSizeClassMapper(prefixCls, Xs, "xs");
            SetSizeClassMapper(prefixCls, Sm, "sm");
            SetSizeClassMapper(prefixCls, Md, "md");
            SetSizeClassMapper(prefixCls, Lg, "lg");
            SetSizeClassMapper(prefixCls, Xl, "xl");
            SetSizeClassMapper(prefixCls, Xxl, "xxl");
        }

        private void SetSizeClassMapper(string prefixCls, OneOf<int, EmbeddedProperty> parameter, string sizeName)
        {
            parameter.Switch(strNum =>
            {
                ClassMapper.If($"{prefixCls}-{sizeName}-{strNum}", () => strNum > 0);
            }, embedded =>
            {
                ClassMapper
                    .If($"{prefixCls}-{sizeName}-{embedded.Span.Value}", () => embedded.Span.Value != null)
                    .If($"{prefixCls}-{sizeName}-order-{embedded.Order.Value}", () => embedded.Order.Value != null)
                    .If($"{prefixCls}-{sizeName}-offset-{embedded.Offset.Value}", () => embedded.Offset.Value != null)
                    .If($"{prefixCls}-{sizeName}-push-{embedded.Push.Value}", () => embedded.Push.Value != null)
                    .If($"{prefixCls}-{sizeName}-pull-{embedded.Pull.Value}", () => embedded.Pull.Value != null);
            });
        }

        private void SetHostFlexStyle()
        {
            if (this.Flex.Value == null && this.Row.Cols.All(x => x.Flex.Value == null))
                return;
            var prefixStyle = "flex:";
            if (this.Flex.Value == null)
            {
                FlexStyle = $"{prefixStyle} 1 1 auto;";
                return;
            }
            FlexStyle = this.Flex.Match(str =>
                {
                    if (Regex.Match(str, "^\\d+(\\.\\d+)?(px|em|rem|%)$").Success)
                    {
                        return $"{prefixStyle} 0 0 {Flex.Value};";
                    }

                    return $"{prefixStyle} {Flex.Value};";
                },
                num => $"{prefixStyle} {Flex.Value} {Flex.Value} auto;");
        }

        private string GetStyle()
        {
            var styleMapper = new ClassMapper();
            styleMapper
                .If(Style, () => string.IsNullOrWhiteSpace(Style) == false)
                .If(GutterStyle, () => string.IsNullOrWhiteSpace(GutterStyle) == false)
                .If(FlexStyle, () => string.IsNullOrWhiteSpace(FlexStyle) == false)
                ;

            return styleMapper.Class;
        }

        protected override void OnInitialized()
        {
            this.Row?.Cols.Add(this);

            this.SetHostClassMap();
            this.SetHostFlexStyle();

            base.OnInitialized();
        }

        protected override void Dispose(bool disposing)
        {
            this.Row?.Cols.Remove(this);

            base.Dispose(disposing);
        }
    }
}
