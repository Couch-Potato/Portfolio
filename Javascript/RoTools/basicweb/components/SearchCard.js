import { Card } from "./Card.js";

export class SearchCard extends Card {

    Body = `<form method="get"><div class="form-group row">
    <div class="col-sm-9"><input type="text" name="q" class="form-control form-control-md" placeholder="Use a ServerId or a Player Username" aria-label="Username"> </div>
                            <label class="col-sm-3"><button type="submit" class="btn btn-secondary btn-sm">Lookup</button></label>
                            
                          </div> </form>
                          
                          `
}

export class PlayerSearchCard extends Card {

  Body = `<form method="get"><div class="form-group row">
    <div class="col-sm-9"><input type="text" name="q" class="form-control form-control-md" placeholder="Enter a Player Username or User Id" aria-label="Username"> </div>
                            <label class="col-sm-3"><button type="submit" class="btn btn-secondary btn-sm">Lookup</button></label>
                            
                          </div> </form>
                          
                          `
}