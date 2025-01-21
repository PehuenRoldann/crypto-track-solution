using Gdk;
using Gtk;
using Pango;
using System;
using System.ComponentModel.DataAnnotations;

namespace CryptoTrackApp.src.view.Components
{
    public class CryptoTreeViewComponent : ScrolledWindow
    {
        private ListStore listStore;
         // Delegado para manejar el evento de fila activada con la información de la fila
        public event EventHandler<CryptoRowActivatedEventArgs> RowActivatedEvent;

        public CryptoTreeViewComponent(int width = 100, int heigh = 500)
        {
            // Configurar las políticas de desplazamiento
            this.SetPolicy(PolicyType.Never, PolicyType.Automatic);

            // Inicializar el ListStore
            this.listStore = new ListStore(typeof(Pixbuf), typeof(string), typeof(int), typeof(double), typeof(float), typeof(float));

            // Inicializar el TreeView con el ListStore
            var treeView = new TreeView(listStore);

            // Configurar las columnas del TreeView
            var iconRenderer = new CellRendererPixbuf();
            var nameRenderer = new CellRendererText();
            var rankRenderer = new CellRendererText();
            var usdPriceRenderer = new CellRendererText();
            var tendencyRenderer = new CellRendererText();
            var notificationRenderer = new CellRendererText();

            notificationRenderer.Editable = true;
            notificationRenderer.Edited += UmbralEditedEventHandler;

            TreeViewColumn iconColumn = new TreeViewColumn("Icon", iconRenderer);
            iconColumn.AddAttribute(iconRenderer, "pixbuf", 0);
            TreeViewColumn nameColumn = new TreeViewColumn("Name", nameRenderer, "text", 1);
            TreeViewColumn rankColumn = new TreeViewColumn("Rank", rankRenderer, "text", 2);
            TreeViewColumn usdColumn = new TreeViewColumn("$USD", usdPriceRenderer, "text", 3);
            TreeViewColumn tendencyColumn = new TreeViewColumn("Tendency", tendencyRenderer, "text", 4);
            TreeViewColumn notificationColumn = new TreeViewColumn("Notification Umbral", notificationRenderer, "text", 5);
            

            // Estilizar columnas y celdas
            foreach (TreeViewColumn item in new TreeViewColumn[]
            {nameColumn, rankColumn, usdColumn, tendencyColumn, notificationColumn})
            {
                item.Alignment = 0.50F;
                item.MinWidth = 50;
            }

            foreach (CellRendererText item in new CellRendererText[]
            {nameRenderer, rankRenderer, usdPriceRenderer, tendencyRenderer, notificationRenderer})
            {
                item.FontDesc = Pango.FontDescription.FromString("Arimo italic 14");
                item.SetPadding(12, 5);
                item.Alignment = Pango.Alignment.Center;

            }

            // Añadir las columnas al TreeView
            treeView.AppendColumn(iconColumn);
            treeView.AppendColumn(nameColumn);
            treeView.AppendColumn(rankColumn);
            treeView.AppendColumn(usdColumn);
            treeView.AppendColumn(tendencyColumn);
            treeView.AppendColumn(notificationColumn);

            // Conectar el evento RowActivated
            treeView.RowActivated += OnRowActivated;

            treeView.Hexpand = true;
            treeView.Vexpand = true;
            // Asegurarse de que el TreeView se expanda completamente
            treeView.Hexpand = true;
            treeView.Vexpand = true;
            // Añadir el TreeView al ScrolledWindow
            this.Add(treeView);
            /* Viewport viewport = new Viewport();
            viewport.Add(treeView);

            viewport.SetSizeRequest(500, 200);
            this.Add(viewport);
 */

            // Asegurarse de que el ScrolledWindow return treeView;se expanda completamente
            this.Hexpand = true;
            this.Vexpand = true;
            this.Valign = Align.Center;
            this.SetSizeRequest(width, heigh);
        }

        public void AddData(Pixbuf icon, string name, int rank, double usdPrice, float tendency, float notificationUmbral)
        { 
            // Agregar nuevos datos al ListStore
            listStore.AppendValues(icon, name, rank, Math.Round(usdPrice, 2), Math.Round(tendency, 2), Math.Round(notificationUmbral, 2));
        }

        private void UmbralEditedEventHandler (object sender, EditedArgs e) {
            // Actualizar el modelo con el nuevo valor
            TreeIter iter;
            float parsedValue = 0;
            bool canParse = float.TryParse(e.NewText, out parsedValue);
            if (listStore.GetIterFromString(out iter, e.Path) && canParse)
            {
                listStore.SetValue(iter, 5, parsedValue);
                Console.WriteLine($"Celda editada: {e.NewText}");
            }
            else {
                Console.WriteLine("VALOR INTRODUCIDO NO VÁLIDO");
            }
        }

        private void OnRowActivated(object sender, RowActivatedArgs args)
        {
            // Recuperar la información de la fila activada
            if (listStore.GetIter(out TreeIter iter, args.Path))
            {
                string name = (string)listStore.GetValue(iter, 1);
                int rank = (int)listStore.GetValue(iter, 2);
                double usdPrice = (double)listStore.GetValue(iter, 3);
                float tendency = (float)listStore.GetValue(iter, 4);

                Console.WriteLine($"Recuperada línea con name: {name}");
                // Crear un objeto personalizado con los datos de la fila activada
                var eventArgs = new CryptoRowActivatedEventArgs( name, rank, usdPrice, tendency);

                // Propagar el evento al contenedor padre con la información de la fila activada
                RowActivatedEvent?.Invoke(this, eventArgs);
            }
        }



        private void OnNotificationEdited(object o, EditedArgs args)
        {
            if (listStore.GetIterFromString(out TreeIter iter, args.Path))
            {
                // Actualizar el valor del umbral en el modelo
                listStore.SetValue(iter, 5, args.NewText);
                Console.WriteLine($"Umbral de notificación actualizado a: {args.NewText}");
            }
        }

        // Configura el modelo para el ComboBox
        private static ListStore CreateComboBoxModel()
        {
            var comboModel = new ListStore(typeof(string));
            comboModel.AppendValues("0.50");
            comboModel.AppendValues("1.00");
            comboModel.AppendValues("1.50");
            comboModel.AppendValues("2.00");
            return comboModel;
        }


    }

    // Clase para los argumentos del evento personalizado
    public class CryptoRowActivatedEventArgs : EventArgs
    {
        public Pixbuf Icon { get; }
        public string Name { get; }
        public int Rank { get; }
        public double UsdPrice { get; }
        public float Tendency { get; }

        public CryptoRowActivatedEventArgs(string name, int rank, double usdPrice, float tendency)
        {
            Name = name;
            Rank = rank;
            UsdPrice = usdPrice;
            Tendency = tendency;
        }



    }
}

