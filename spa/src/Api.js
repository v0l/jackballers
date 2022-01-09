export class Api {
    async getCollections() {
        return await this.jsonGet("/api/collection");
    }

    async getCollectionItems(id) {
        return await this.jsonGet(`/api/collection/${id}`);
    }
    
    async getInvoice(item) {
        return await this.jsonGet(`/api/invoice?itemId=${item}`);
    }
    
    async waitForInvoice(id) {
        return await this.jsonGet(`/api/invoice/${id}/wait`);
    }
    
    async jsonGet(uri) {
        let req = await fetch(uri, {
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