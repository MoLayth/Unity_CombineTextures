using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ImageObjectFiled : ObjectField {
    static readonly string ImageObjectFiled_uss = "ImageObjectFiled";
    static readonly string ImageObjectFiled_Label_uss = "ImageObjectFiled-Label";
    static readonly string ImageObjectFiled_previewImage_uss = "ImageObjectFiled-previewImage";

    Image previewImage;

    string _Image_Label;
    Label _label;
    [UxmlAttribute]
    string Image_Label {
        get { return _Image_Label; }
        set {
            _Image_Label = value;
            _label.text = value;
        }
    }
    public ImageObjectFiled() {
        objectType = typeof(Texture2D);
        AddToClassList(ImageObjectFiled_uss);

        _label = new Label("Label");
        _label.AddToClassList(ImageObjectFiled_Label_uss);

        previewImage = new Image();
        previewImage.AddToClassList(ImageObjectFiled_previewImage_uss);
        previewImage.pickingMode = PickingMode.Ignore;
        previewImage.name = "previewImage";

        Add(previewImage);
        Add(_label);

        this.RegisterValueChangedCallback((evt) => {
            if (previewImage == null) return;
            previewImage.style.backgroundImage = evt.newValue as Texture2D;
        });

        this.RegisterCallback<PointerDownEvent>(evt => {
            if(evt.button == 1){
                this.value = null;
                previewImage.style.backgroundImage = null;
            }
        });
    }
}
