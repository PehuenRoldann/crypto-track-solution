using Gtk;
using System;
using System.Collections.Generic;
using CryptoTrackApp.src.utils;

namespace CryptoTrackApp.src.view.components
{
    public class AboutConfigDialog : Dialog
    {
        private readonly Dictionary<string, Entry> _entries = new();
        private readonly Dictionary<string, string> _labelMap = new()
        {
            { ConfigurationsKeys.CoinCapApi, "URL de la API de CoinCap" },
            { ConfigurationsKeys.ApiKey, "Clave de API de CoinCap" },
            { ConfigurationsKeys.DbConnection, "Conexión a base de datos" },
            { ConfigurationsKeys.MainViewTimeoutRefresh, "Tiempo de refresco (m)" }
        };
        private readonly IConfigService _configService;

        public AboutConfigDialog(Gtk.Window parent)
        {
            this.Title = "Acerca de CryptoTrack";
            this.TransientFor = parent;
            this.SetDefaultSize(500, 400);
            this.Modal = true;

            _configService = JsonConfigService.GetInstance();

            var scroll = new ScrolledWindow();
            scroll.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            var configBox = new VBox(false, 10) { BorderWidth = 10 };

            // Campos de configuración editables
            foreach (var key in _labelMap.Keys)
            {
                AddConfigField(configBox, key);
            }

            // Sección de información
            var infoLabel = new Label("\nCryptoTrack es una aplicación para visualizar en tiempo real información de criptomonedas, incluyendo:")
            {
                Justify = Justification.Left,
                LineWrap = true
            };
            configBox.PackStart(infoLabel, false, false, 10);

            var bullets = new Label("\u2022 Precio en USD\n\u2022 Porcentaje de cambio en 24hs\n\u2022 Ranking general\n\nConfiguración disponible:\n- Tiempo de refresco\n- API URL y Key\n- Conexión a base de datos")
            {
                Justify = Justification.Left,
                LineWrap = true
            };
            configBox.PackStart(bullets, false, false, 10);

            scroll.AddWithViewport(configBox);
            this.ContentArea.PackStart(scroll, true, true, 0);

            // Crear botones manualmente para aplicar clases CSS
            var buttonBox = new HButtonBox { Layout = ButtonBoxStyle.Center, Spacing = 10 };

            var saveButton = new Button("Guardar");
            var cancelButton = new Button("Cancelar");

            saveButton.StyleContext.AddClass("dialog-button");
            cancelButton.StyleContext.AddClass("dialog-button");

            // Opcional: establecer propiedades específicas desde código
            saveButton.StyleContext.AddProvider(CreateCssProvider("dialog-button"), 600);
            cancelButton.StyleContext.AddProvider(CreateCssProvider("dialog-button"), 600);

            saveButton.Clicked += (sender, args) => {
                foreach (var kvp in _entries)
                {
                    _configService.SetKey(kvp.Key, kvp.Value.Text);
                }
                this.Destroy();
            };

            cancelButton.Clicked += (sender, args) => this.Destroy();

            buttonBox.Add(saveButton);
            buttonBox.Add(cancelButton);

            this.ContentArea.PackStart(buttonBox, false, false, 10);

            this.ShowAll();
        }

        private void AddConfigField(Box parent, string key)
        {
            string? value = _configService.GetString(key);
            string labelText = _labelMap.ContainsKey(key) ? _labelMap[key] : key;

            var label = new Label($"{labelText}:") { Xalign = 0 };
            var entry = new Entry { Text = value ?? "" };
            _entries[key] = entry;

            parent.PackStart(label, false, false, 0);
            parent.PackStart(entry, false, false, 5);
        }

        private CssProvider CreateCssProvider(string className)
        {
            var css = new CssProvider();
            css.LoadFromData($@".{className} {{
                background-color: #3a3f58;
                color: white;
                border-radius: 6px;
                padding: 6px 12px;
            }}");
            return css;
        }
    }
}
