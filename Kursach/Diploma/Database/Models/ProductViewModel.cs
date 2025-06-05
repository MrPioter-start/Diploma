using System.ComponentModel;

public class ProductViewModel : INotifyPropertyChanged
{
    private int _quantity;
    public int ProductID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    public decimal Total => Price * Quantity;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}