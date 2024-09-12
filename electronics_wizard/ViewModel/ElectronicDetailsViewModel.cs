using electronics_wizard.Models;

namespace electronics_wizard.ViewModel
{
    public class ElectronicDetailsViewModel
    {
        public required Electronics Electronics { get; set; }
        public required List<Electronics> OtherElectronics { get; set; }
    }
}
