window.mapHelper = {
    maps: {},
    
    initMap: function(mapId, latitude, longitude, zoom, editable) {
        // Remove existing map if any
        if (this.maps[mapId]) {
            this.maps[mapId].remove();
        }
        
        // Create map
        const map = L.map(mapId).setView([latitude, longitude], zoom);
        
        // Add tile layer
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            maxZoom: 19
        }).addTo(map);
        
        // Add marker
        const marker = L.marker([latitude, longitude], {
            draggable: editable
        }).addTo(map);
        
        // Store map and marker
        this.maps[mapId] = {
            map: map,
            marker: marker
        };
        
        return {
            latitude: latitude,
            longitude: longitude
        };
    },
    
    updateMarker: function(mapId, latitude, longitude) {
        if (this.maps[mapId]) {
            const marker = this.maps[mapId].marker;
            marker.setLatLng([latitude, longitude]);
            this.maps[mapId].map.setView([latitude, longitude]);
        }
    },
    
    getMarkerPosition: function(mapId) {
        if (this.maps[mapId]) {
            const pos = this.maps[mapId].marker.getLatLng();
            return {
                latitude: pos.lat,
                longitude: pos.lng
            };
        }
        return null;
    },
    
    onMarkerDragEnd: function(mapId, dotNetHelper) {
        if (this.maps[mapId]) {
            this.maps[mapId].marker.on('dragend', function(e) {
                const pos = e.target.getLatLng();
                dotNetHelper.invokeMethodAsync('OnMarkerMoved', pos.lat, pos.lng);
            });
        }
    },
    
    onMapClick: function(mapId, dotNetHelper) {
        if (this.maps[mapId]) {
            this.maps[mapId].map.on('click', function(e) {
                dotNetHelper.invokeMethodAsync('OnMapClicked', e.latlng.lat, e.latlng.lng);
            });
        }
    },
    
    destroyMap: function(mapId) {
        if (this.maps[mapId]) {
            this.maps[mapId].map.remove();
            delete this.maps[mapId];
        }
    }
};
