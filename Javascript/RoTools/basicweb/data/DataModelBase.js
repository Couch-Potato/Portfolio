export class DataModelBase {
    InnerRecord;
    IsOpened = false;
    async Save(){
        await this.InnerRecord.save();
    }
}
