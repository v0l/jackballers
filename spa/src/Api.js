export class Api {
    async getInvoice(item) {
        let req = await fetch(`/api/invoice?item=${item}`, {
            headers: {
                "accept": "application/json"
            }
        });
        
        if(req.ok) {
            return await req.json();
        }
        return null;
    }
}