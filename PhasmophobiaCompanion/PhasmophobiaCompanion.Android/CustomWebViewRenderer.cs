﻿using Android.Content;
using Android.Webkit;
using Java.Lang;
using PhasmophobiaCompanion.Droid;
using PhasmophobiaCompanion.Models;
using Serilog;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using WebView = Xamarin.Forms.WebView;

[assembly: ExportRenderer(typeof(WebView), typeof(CustomWebViewRenderer))]

namespace PhasmophobiaCompanion.Droid
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        public CustomWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
                try
                {
                    Control.SetBackgroundColor(Color.Transparent);
                    Control.Settings.JavaScriptEnabled = true;
                    Control.Settings.AllowUniversalAccessFromFileURLs = true;
                    var css = @"<style>
                                @font-face {
                                    font-family: 'CustomFont';
                                    src: url('file:///android_asset/Overpass_Regular.ttf') format('truetype');
                                }
                                * {
                                    font-family: 'CustomFont', sans-serif;
                                    font-size: 16px;
                                    color: #444;
                                }
                                table {
                                  border-radius: 3px;
                                  border: 1px solid black;
                                  font-size: 14px;
                                  text-align: center;
                                }

                                th, td {
                                  border-radius: 3px;
                                  border: 1px solid black;
                                  font-size: 14px;
                                  text-align: center;
                                }
                            </style>";

                    Control.SetBackgroundColor(Color.Transparent);
                    Control.Settings.AllowUniversalAccessFromFileURLs = true;
                    Control.LoadDataWithBaseURL(null, $"{css}{((HtmlWebViewSource) Element.Source).Html}", "text/html",
                        "utf-8", null);
                    Control.SetWebViewClient(new WebViewClient(this));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка при настройке WebView.");
                }
        }

        private class WebViewClient : Android.Webkit.WebViewClient
        {
            private readonly CustomWebViewRenderer renderer;

            public WebViewClient(CustomWebViewRenderer renderer)
            {
                this.renderer = renderer;
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                base.OnPageFinished(view, url);
                try
                {
                    view.EvaluateJavascript("document.body.scrollHeight", new ValueCallback(renderer));
                    if (renderer.Element.BindingContext is UnfoldingItem item)
                        item.IsExpanded = false;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка при выполнении JavaScript или обработке после загрузки страницы.");
                }
            }
        }

        private class ValueCallback : Object, IValueCallback
        {
            private readonly CustomWebViewRenderer renderer;

            public ValueCallback(CustomWebViewRenderer renderer)
            {
                this.renderer = renderer;
            }

            public void OnReceiveValue(Object value)
            {
                try
                {
                    if (value != null && int.TryParse(value.ToString(), out var height))
                        renderer.Element.HeightRequest = height;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Ошибка при установке высоты WebView.");
                }
            }
        }
    }
}