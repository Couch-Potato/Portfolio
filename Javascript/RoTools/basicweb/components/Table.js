import { Renderable } from "./Renderable.js";
import {RandomString } from "../util.js";
export class Table extends Renderable{
  /**
   * @type {Renderable | string | undefined}
   */
    Footer;
    Rows = []
    Columns = []
    Class = "table table-hover"
    /**
     * Creates a new table
     * @param {Array<String>} cols The heading name of the columns.
     */
    constructor(cols){
        super();
        this.Columns = cols;
    }
    Render(){
        var dx ="";
        for (var x of this.Rows)
        {
            dx+=x.Render()
        }
        var cx = "";
        for (var x of this.Columns){
            cx+=`<th>${x}</th>`
        }
        return `<table ${this.Id? `id="${this.Id}"`:""} class="${this.Class}">
                    <thead>
                      <tr>
                    ${cx}
                      </tr>
                    </thead>
                    <tbody>
                      ${dx}
                    </tbody>
                  </table>${this.Footer ? `<div class="card-footer">${this.Footer.Render()}</div>` : ''}`
    }
    AddRow(row)
    {
        this.Rows.push(row)
    }
    GetChildren(){
        return this.Rows;
    }
}
export class TableRow extends Renderable {
    Data = []
    /**
     * Creates a new row.
     * @param {Array<String | Renderable>} cols The content of the row.
     */
    constructor(cols) {
        super();
        this.Data = cols;
    }
    Render(){
        this.PrepareData()
        var dx = "";
        for (var x of this.Data)
        {
          if (x){
            if (x.Renderable)
              dx += `<td>${x.Render()}</td>`;
            else
              dx += `<td>${x}</td>`;
          }
            
        }
            
        return `
        <tr>${dx}</tr>
        `
    }
    GetChildren(){
      var chdTable = [];
      for (var x of this.Data){
        if (x) {
          if (x.Renderable) {
            chdTable.push(x);
          }
        }
        
      }
      return chdTable;
    }
    PrepareData(){

    }
}

export class DataTable extends Table {
    Class = "table dataTable table-hover no-footer"
    constructor(cols,id){
        super(cols)
        if (!id) {
           id = RandomString(10); 
        }
        this.Id = id;
        this.Scripts.push("/assets/vendors/datatables.net/jquery.dataTables.js")
        this.Scripts.push("/assets/vendors/datatables.net-bs4/dataTables.bootstrap4.js")
        this.CSS.push("/assets/vendors/datatables.net-bs4/dataTables.bootstrap4.css");
        this.CSS.push("/assets/vendors/datatables.net-fixedcolumns-bs4/fixedColumns.bootstrap4.min.css")
        this.InlineScripts.push(` $(function () {
    $('#${id}').DataTable({
      "aLengthMenu": [
        [5, 10, 15, -1],
        [5, 10, 15, "All"]
      ],
      "iDisplayLength": 5,
      "bLengthChange": false,
      "language": {
        search: "Search :"
      }
    });
    $('#order-listing').each(function () {
      var datatable = $(this);
      // SEARCH - Add the placeholder for Search and Turn this into in-line form control
      var search_input = datatable.closest('.dataTables_wrapper').find('div[id$=_filter] input');
      search_input.attr('placeholder', 'Sort');
      // search_input.removeClass('form-control-sm');
     
    });
  });`)
    }
}