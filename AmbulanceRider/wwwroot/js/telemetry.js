// Telemetry and Geolocation Helper Functions

window.getLocation = function () {
    return new Promise((resolve, reject) => {
        if (!navigator.geolocation) {
            reject(new Error('Geolocation is not supported'));
            return;
        }

        navigator.geolocation.getCurrentPosition(
            (position) => {
                resolve({
                    coords: {
                        latitude: position.coords.latitude,
                        longitude: position.coords.longitude,
                        accuracy: position.coords.accuracy,
                        altitude: position.coords.altitude,
                        altitudeAccuracy: position.coords.altitudeAccuracy,
                        heading: position.coords.heading,
                        speed: position.coords.speed
                    },
                    timestamp: position.timestamp
                });
            },
            (error) => {
                // Don't reject, just resolve with null
                // This prevents blocking the main flow if location is denied
                console.warn('Location access denied or unavailable:', error.message);
                resolve(null);
            },
            {
                enableHighAccuracy: true,
                timeout: 5000,
                maximumAge: 0
            }
        );
    });
};

window.watchLocation = function (dotNetHelper, methodName) {
    if (!navigator.geolocation) {
        console.warn('Geolocation is not supported');
        return -1;
    }

    const watchId = navigator.geolocation.watchPosition(
        (position) => {
            dotNetHelper.invokeMethodAsync(methodName, {
                coords: {
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                    accuracy: position.coords.accuracy,
                    altitude: position.coords.altitude,
                    altitudeAccuracy: position.coords.altitudeAccuracy,
                    heading: position.coords.heading,
                    speed: position.coords.speed
                },
                timestamp: position.timestamp
            });
        },
        (error) => {
            console.warn('Location watch error:', error.message);
        },
        {
            enableHighAccuracy: true,
            timeout: 10000,
            maximumAge: 0
        }
    );

    return watchId;
};

window.clearLocationWatch = function (watchId) {
    if (watchId && navigator.geolocation) {
        navigator.geolocation.clearWatch(watchId);
    }
};

// Get battery information
window.getBatteryInfo = async function () {
    try {
        if ('getBattery' in navigator) {
            const battery = await navigator.getBattery();
            return {
                level: battery.level,
                charging: battery.charging,
                chargingTime: battery.chargingTime,
                dischargingTime: battery.dischargingTime
            };
        }
    } catch (error) {
        console.warn('Battery API not available:', error);
    }
    return null;
};

// Get network information
window.getNetworkInfo = function () {
    try {
        if ('connection' in navigator || 'mozConnection' in navigator || 'webkitConnection' in navigator) {
            const connection = navigator.connection || navigator.mozConnection || navigator.webkitConnection;
            return {
                effectiveType: connection.effectiveType,
                downlink: connection.downlink,
                rtt: connection.rtt,
                saveData: connection.saveData,
                type: connection.type
            };
        }
    } catch (error) {
        console.warn('Network Information API not available:', error);
    }
    return null;
};

// Get device memory (if available)
window.getDeviceMemory = function () {
    try {
        if ('deviceMemory' in navigator) {
            return navigator.deviceMemory;
        }
    } catch (error) {
        console.warn('Device Memory API not available:', error);
    }
    return null;
};

// Get hardware concurrency (CPU cores)
window.getHardwareConcurrency = function () {
    try {
        if ('hardwareConcurrency' in navigator) {
            return navigator.hardwareConcurrency;
        }
    } catch (error) {
        console.warn('Hardware Concurrency API not available:', error);
    }
    return null;
};

// Get account information (Google/Apple)
window.getAccountInfo = async function () {
    try {
        // Note: For web apps, we cannot directly access Google/Apple account info
        // This would require OAuth integration or native app capabilities
        // For now, we detect the OS and return the account type
        
        const userAgent = navigator.userAgent;
        let accountType = 'None';
        
        if (/Android/i.test(userAgent)) {
            accountType = 'Google';
        } else if (/iPhone|iPad|iPod|Macintosh/i.test(userAgent)) {
            accountType = 'Apple';
        }
        
        return {
            googleAccount: null,  // Would need OAuth to get actual email
            appleAccount: null,   // Would need Sign in with Apple
            accountType: accountType
        };
    } catch (error) {
        console.warn('Account info not available:', error);
    }
    return null;
};

// Get installed apps (limited in web context)
window.getInstalledApps = async function () {
    try {
        // In a web browser, we cannot access the list of installed apps
        // This would require:
        // 1. Native mobile app (Cordova, Capacitor, React Native, etc.)
        // 2. Browser extension
        // 3. PWA with specific permissions (very limited)
        
        // For PWA, we can check if certain apps are installed via related applications
        if ('getInstalledRelatedApps' in navigator) {
            const relatedApps = await navigator.getInstalledRelatedApps();
            if (relatedApps && relatedApps.length > 0) {
                const appsList = relatedApps.map(app => ({
                    platform: app.platform,
                    url: app.url,
                    id: app.id
                }));
                
                return {
                    appsJson: JSON.stringify(appsList),
                    count: appsList.length
                };
            }
        }
        
        // Fallback: return browser plugins/extensions count (very limited info)
        if (navigator.plugins && navigator.plugins.length > 0) {
            const plugins = Array.from(navigator.plugins).map(plugin => plugin.name);
            return {
                appsJson: JSON.stringify(plugins.slice(0, 20)), // Limit to 20 to avoid huge payload
                count: navigator.plugins.length
            };
        }
    } catch (error) {
        console.warn('Installed apps info not available:', error);
    }
    return null;
};

// Get app version from meta tag or manifest
window.getAppVersion = function () {
    try {
        // Try to get from meta tag
        const versionMeta = document.querySelector('meta[name="app-version"]');
        if (versionMeta) {
            return versionMeta.getAttribute('content');
        }
        
        // Try to get from manifest
        const manifestLink = document.querySelector('link[rel="manifest"]');
        if (manifestLink) {
            fetch(manifestLink.href)
                .then(response => response.json())
                .then(manifest => {
                    return manifest.version || '1.0.0';
                })
                .catch(() => '1.0.0');
        }
    } catch (error) {
        console.warn('App version not available:', error);
    }
    return '1.0.0';
};
