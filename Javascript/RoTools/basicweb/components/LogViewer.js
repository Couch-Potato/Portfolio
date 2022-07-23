import { Column } from "./Column.js";
import { MarkdownView } from "./MarkdownView.js";
import { Modal, ModalSize } from "./Modal.js";
import { ObjectViewer } from "./ObjectViewer.js";
import { Row } from "./Row.js";
import { StackTrace } from "./StackTrace.js";


export class LogViewer extends Modal{
    /**
     * @type {StackTrace}
     */
    StackView;

    /**
     * @type {ObjectViewer}
     */
    ObjectView;

    /**
     * @type {MarkdownView}
     */
    MarkdownView;
    constructor(title, stack, object, markdown, type, typeColor)
    {
        super(title);
        this.Size = ModalSize.Regular;

        this.StackView = new StackTrace(stack);
        this.ObjectView = new ObjectViewer(object);
        this.MarkdownView = new MarkdownView(markdown);

        var TopRow = new Row();
        var BottomRow = new Row();

        var LeftCol = new Column(7);
        var TopCol = new Column(10);
        var RightCol = new Column(5);
        LeftCol.StretchCard = false;
        LeftCol.GridMargin = false;

        RightCol.StretchCard = false;
        RightCol.GridMargin = false;

        TopCol.StretchCard = false;
        TopCol.GridMargin = false;
        

        /**
         * TOP AREA
         */
        TopCol.AddElement(this.MarkdownView);
        TopRow.AddElement(TopCol);

        /**
         * BOTTOM AREA
         */
        LeftCol.AddElement(this.StackView);
        LeftCol.SetCSS("border-right", "1px solid rgba(0, 0, 0, 0.1)")

        RightCol.AddElement(this.ObjectView);

        BottomRow.AddElement(LeftCol);
        BottomRow.AddElement(RightCol);


        var TRLabel = new Row();
        var TRCol = new Column(10);
        TRCol.AddElement(`<h2>${title} <label class="badge badge-${typeColor}" style="vertical-align: middle;">${type}</label></h2>`)
        TRLabel.AddElement(TRCol);

        var BRLabel = new Row();
        var BR1 = new Column(7);
        var BR2 = new Column(5);
        BR1.AddElement("<h3>Stack Trace</h3>")
        BR2.AddElement("<h3>Object Data</h3>")
        BRLabel.AddElement(BR1);
        BRLabel.AddElement(BR2);

        this.Children = [
            TRLabel,
            "<hr/>",
            TopRow,
            "<hr/>",
            BRLabel,
            BottomRow
        ]
    }
}