using CryptoTrackApp.src.utils;
using Gdk;
using Gtk;
using Pango;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace CryptoTrackApp.src.view.Components
{
    public class CryptoTreeViewComponent : ScrolledWindow
    {
        private ListStore listStore;
         // Delegado para manejar el evento de fila activada con la información de la fila
        public event EventHandler<CryptoRowActivatedEventArgs> RowActivatedEvent;
        public event EventHandler<UnfollowEventArgs> UnfollowEvent;
        public event EventHandler<NotificationEditedEventArgs> NotificationEditedEvent;

        public CryptoTreeViewComponent(int width = 100, int heigh = 500)
        {
            // Configurar las políticas de desplazamiento
            this.SetPolicy(PolicyType.Never, PolicyType.Automatic);

            // Inicializar el ListStore
            this.listStore = new ListStore(
                typeof(Pixbuf), typeof(string), typeof(int), typeof(string), 
                typeof(string), typeof(string), 
                typeof(Pixbuf), typeof(string));

            // Inicializar el TreeView con el ListStore
            var treeView = new TreeView(listStore);

            // Configurar las columnas del TreeView
            var iconRenderer = new CellRendererPixbuf();
            var nameRenderer = new CellRendererText();
            var rankRenderer = new CellRendererText();
            var usdPriceRenderer = new CellRendererText();
            var tendencyRenderer = new CellRendererText();
            var notificationRenderer = new CellRendererText();
            var unfollowBtnRenderer = new CellRendererPixbuf();
            // var idRenderer = new CellRendererText();
            

            notificationRenderer.Editable = true;
            notificationRenderer.Edited += UmbralEditedEventHandler;

            


            TreeViewColumn iconColumn = new TreeViewColumn("Icon", iconRenderer);
            iconColumn.AddAttribute(iconRenderer, "pixbuf", 0);
            TreeViewColumn nameColumn = new TreeViewColumn("Name", nameRenderer, "text", 1);
            TreeViewColumn rankColumn = new TreeViewColumn("Rank", rankRenderer, "text", 2);
            TreeViewColumn usdColumn = new TreeViewColumn("$USD", usdPriceRenderer, "text", 3);
            TreeViewColumn tendencyColumn = new TreeViewColumn("Tendency", tendencyRenderer, "text", 4);
            TreeViewColumn notificationColumn = new TreeViewColumn("Notification\nUmbral", notificationRenderer, "text", 5);
            TreeViewColumn unfollowBtnColumn = new TreeViewColumn("Unfollow", unfollowBtnRenderer, "pixbuf", 6);
            // TreeViewColumn idColumn = new TreeViewColumn("Id", idRenderer, "text", 8);
            
            

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
            treeView.AppendColumn(unfollowBtnColumn);

            // Conectar el evento RowActivated
            treeView.RowActivated += OnRowActivated;
            treeView.ButtonReleaseEvent += OnButtonReleaseEvent;

            treeView.Hexpand = true;
            treeView.Vexpand = true;
            // Asegurarse de que el TreeView se expanda completamente
            treeView.Hexpand = true;
            treeView.Vexpand = true;
            // Añadir el TreeView al ScrolledWindow
            this.Add(treeView);

            // Asegurarse de que el ScrolledWindow return treeView;se expanda completamente
            this.Hexpand = true;
            this.Vexpand = true;
            this.Valign = Align.Center;
            this.SetSizeRequest(width, heigh);
        }



        private void OnButtonReleaseEvent(object sender, ButtonReleaseEventArgs args)
        {
            if (args.Event.Button == 1) // Verifica que sea un clic izquierdo
            {
                var treeView = sender as TreeView;
                if (treeView!.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out TreePath path, out TreeViewColumn column, out int cell_x, out int cell_y))
                {
                    // Verifica si el clic fue en la columna del Pixbuf
                    if (column.Title == "Unfollow")
                    {
                        // Obtén la información de la fila
                        if (listStore.GetIter(out TreeIter iter, path))
                        {
                            Pixbuf icon = (Pixbuf)listStore.GetValue(iter, 0);
                            string name = (string)listStore.GetValue(iter, 1);
                            string currencyId = (string)listStore.GetValue(iter, 7);

                            // Lanza el evento personalizado
                            UnfollowEvent?.Invoke(this, new UnfollowEventArgs(currencyId, name, icon));
                        }

                        // Evita que otros eventos (como RowActivated) se disparen
                        args.RetVal = true;
                    }
                }
            }
        }

        public void AddData(Pixbuf icon, string name, int rank, double usdPrice, float tendency, float notificationUmbral, string currencyId)
        { 
            // Agregar nuevos datos al ListStore
            Pixbuf unfollowIcon = Pixbuf.LoadFromResource(IconsPaths.UnfllowIconPath);
            
            listStore.AppendValues(
                icon,
                name,
                rank,
                Math.Round(usdPrice, 2).ToString(),
                Math.Round(tendency, 2).ToString(),
                Math.Round(notificationUmbral, 2).ToString(),
                unfollowIcon,
                currencyId
             );
        }

        private void UmbralEditedEventHandler (object sender, EditedArgs e) {
            // Actualizar el modelo con el nuevo valor
            TreeIter iter;
            float parsedValue = 0;
            bool canParse = float.TryParse(e.NewText, out parsedValue);
            if (listStore.GetIterFromString(out iter, e.Path) && canParse && parsedValue >= 0)
            {
                string name = (string)listStore.GetValue(iter, 1);
                int rank = (int)listStore.GetValue(iter, 2);
                string currencyId = (string)listStore.GetValue(iter, 7);
                string twoDigitsNumber = Math.Round(parsedValue, 2).ToString();
                listStore.SetValue(iter, 5, twoDigitsNumber);
                NotificationEditedEvent?.Invoke(this, new NotificationEditedEventArgs(currencyId, name, rank, parsedValue));
            }
            else {
                var parent = GtkUtils.GetParentWindow(this);
                if (parent != null) {
                    var dialog = new InformationDialog (parent, "Invalid value", pHeight: 400, fontSizePx: 12);
                    dialog.Show();
                    dialog.ShowContent(
                        "CLARIFICATIONS\n"
                        + "* You shouls put a value between 0 and 2.\n" 
                        + "* The value indicates the percentage of difference to report.\n"
                        + "* 0 means not notifications for that subscription.",
                        ImagesArrPaths.Warning
                    );
                }       
            }
        }

        private void OnRowActivated(object sender, RowActivatedArgs args)
        {
            // Recuperar la información de la fila activada
            if (listStore.GetIter(out TreeIter iter, args.Path))
            {
                string name = (string)listStore.GetValue(iter, 1);
                int rank = (int)listStore.GetValue(iter, 2);
                double usdPrice = double.Parse((string)listStore.GetValue(iter, 3));
                float tendency = float.Parse((string)listStore.GetValue(iter, 4));

                // Crear un objeto personalizado con los datos de la fila activada
                var eventArgs = new CryptoRowActivatedEventArgs( name, rank, usdPrice, tendency);

                // Propagar el evento al contenedor padre con la información de la fila activada
                RowActivatedEvent?.Invoke(this, eventArgs);
            }
        }

    }

    // Clase para los argumentos del evento personalizado
    public class CryptoRowActivatedEventArgs : EventArgs
    {
        // public Pixbuf Icon { get; }
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


    public class UnfollowEventArgs : EventArgs
    {
        public string CurrencyId { get; }
        public Pixbuf Icon {get; }
        public string Name { get; }
        public UnfollowEventArgs (string currencyId, string name, Pixbuf icon) 
        {
            CurrencyId = currencyId;
            Name = name;
            Icon = icon;
        }
    }

    public class NotificationEditedEventArgs : EventArgs
    {
        public string CurrencyName { get; }
        public string CurrencyId { get; }
        public int Rank { get; }
        public float UmbralValue { get; }

        public NotificationEditedEventArgs (string currencyId, string currencyName, int rank, float umbralValue){
            CurrencyId = currencyId;
            CurrencyName = currencyName;
            Rank = rank;
            UmbralValue = umbralValue;
        }

    }
}

