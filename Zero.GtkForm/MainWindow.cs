using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace Zero.GtkForm;

internal class MainWindow : Window
{
    [UI] private Label _label1 = null;
    [UI] private Button _button1 = null;

    private Int32 _counter;

    public MainWindow() : this(new Builder("MainWindow.glade")) { }

    private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
    {
        builder.Autoconnect(this);

        DeleteEvent += Window_DeleteEvent;
        _button1.Clicked += Button1_Clicked;
    }

    private void Window_DeleteEvent(Object sender, DeleteEventArgs a)
    {
        Application.Quit();
    }

    private void Button1_Clicked(Object sender, EventArgs a)
    {
        _counter++;
        _label1.Text = "Hello World! This button has been clicked " + _counter + " time(s).";
    }
}
