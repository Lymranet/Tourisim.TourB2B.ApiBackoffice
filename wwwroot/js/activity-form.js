let map = null;
let marker = null;
let currentStep = 1;
const totalSteps = 6;
let categoryCounter = 1;
let timeSlotCounter = 0;
let meetingPointCounter = 0;
let meetingPointMarkers = [];
let locationMap = null;
let locationMarker = null;
let meetingPointMap = null;
let meetingPointModal = null;

const subcategories = {
    'Adventure': ['Hiking', 'Rock Climbing', 'Rafting', 'Kayaking', 'Camping', 'Zip-lining', 'Paragliding'],
    'Cultural': ['Museum Tours', 'Historical Sites', 'Art Galleries', 'Local Crafts', 'Traditional Shows', 'Cooking Classes'],
    'Nature': ['Wildlife Watching', 'Bird Watching', 'National Parks', 'Botanical Gardens', 'Photography Tours'],
    'Sports': ['Surfing', 'Skiing', 'Cycling', 'Golf', 'Tennis', 'Horse Riding'],
    'Food': ['Food Tours', 'Wine Tasting', 'Beer Tours', 'Local Markets', 'Restaurant Hopping'],
    'Wellness': ['Yoga', 'Spa', 'Meditation', 'Fitness Classes', 'Thermal Springs']
};

document.addEventListener('DOMContentLoaded', function() {
    showStep(1);
    initializeEventListeners();
    initMeetingPointMap();
});

function initializeEventListeners() {
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const createForm = document.getElementById('createForm');

    if (nextBtn) nextBtn.addEventListener('click', handleNext);
    if (prevBtn) prevBtn.addEventListener('click', handlePrev);
    if (createForm) createForm.addEventListener('submit', handleSubmit);
}

function handleNext() {
    if (validateCurrentStep() && currentStep < totalSteps) {
        showStep(currentStep + 1);
    }
}

function handlePrev() {
    if (currentStep > 1) {
        showStep(currentStep - 1);
    }
}

function showStep(step) {
    const steps = document.querySelectorAll('.form-step');
    steps.forEach(function(el) { 
        el.style.display = 'none';
    });

    const currentStepElement = document.getElementById(`step${step}`);
    if (currentStepElement) {
        currentStepElement.style.display = 'block';
    }

    currentStep = step;
    
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const submitBtn = document.getElementById('submitBtn');

    if (prevBtn) prevBtn.style.display = step === 1 ? 'none' : 'inline-block';
    if (nextBtn) nextBtn.style.display = step === totalSteps ? 'none' : 'inline-block';
    if (submitBtn) submitBtn.style.display = step === totalSteps ? 'inline-block' : 'none';
    
    updateProgressBar();
    updateStepIndicators();

    if (step === 2 && !map) {
        setTimeout(initMap, 200);
    }
}

function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

function validatePhone(phone) {
    const re = /^\+?[\d\s-]{10,}$/;
    return re.test(phone);
}

function validateCurrentStep() {
    const currentStepElement = document.getElementById(`step${currentStep}`);
    if (!currentStepElement) return true;

    const requiredFields = currentStepElement.querySelectorAll('[required]');
    let isValid = true;

    requiredFields.forEach(function(field) {
        // Remove any existing validation classes
        field.classList.remove('is-invalid');
        field.classList.remove('is-valid');

        if (!field.value) {
            field.classList.add('is-invalid');
            isValid = false;
            field.scrollIntoView({ behavior: 'smooth', block: 'center' });
        } else {
            // Specific validation for different field types
            if (field.type === 'email' && !validateEmail(field.value)) {
                field.classList.add('is-invalid');
                isValid = false;
            } else if (field.type === 'tel' && !validatePhone(field.value)) {
                field.classList.add('is-invalid');
                isValid = false;
            } else if (field.type === 'number') {
                const value = parseFloat(field.value);
                const min = parseFloat(field.min);
                const max = parseFloat(field.max);
                
                if ((min !== undefined && value < min) || (max !== undefined && value > max)) {
                    field.classList.add('is-invalid');
                    isValid = false;
                }
            } else {
                field.classList.add('is-valid');
            }
        }
    });

    return isValid;
}

function validateForm() {
    const formData = collectFormData();
    
    // Zorunlu alanları kontrol et
    if (!formData.title || !formData.description || !formData.category || !formData.duration) {
        alert('Please fill in all required basic information fields.');
        return false;
    }
    
    // Konum bilgilerini kontrol et
    if (!formData.location.address || !formData.location.city || !formData.location.country) {
        alert('Please fill in all location fields.');
        return false;
    }
    
    // İletişim bilgilerini kontrol et
    if (!formData.contactInfo.name || !formData.contactInfo.email || !formData.contactInfo.phone) {
        alert('Please fill in all contact information fields.');
        return false;
    }
    
    // Fiyat bilgilerini kontrol et
    if (!formData.priceInfo.basePrice || !formData.priceInfo.currency || 
        !formData.priceInfo.minimumParticipants || !formData.priceInfo.maximumParticipants) {
        alert('Please fill in all pricing information fields.');
        return false;
    }
    
    // Dil seçimini kontrol et
    if (formData.languages.length === 0) {
        alert('Please select at least one language.');
        return false;
    }
    
    // Zaman dilimlerini kontrol et
    if (formData.timeSlots.length === 0) {
        alert('Please add at least one time slot.');
        return false;
    }
    
    // Buluşma noktalarını kontrol et
    if (formData.meetingPoints.length === 0) {
        alert('Please add at least one meeting point.');
        return false;
    }
    
    return true;
}

async function handleSubmit(event) {
    event.preventDefault();
    
    if (!validateForm()) {
        return;
    }
    
    // Loading durumunu göster
    const submitBtn = document.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Saving...';
    
    try {
        const formData = collectFormData();
        console.log('Form data:', formData); // Debug için
        
        // Form verilerini JSON olarak gönder
        const response = await fetch('/Activities/Create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(formData)
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Form submission failed');
        }

        const result = await response.json();
        
        if (result.success) {
            // Başarılı kayıt sonrası aktiviteler listesine yönlendir
            window.location.href = '/Activities';
        } else {
            throw new Error(result.message || 'Form submission failed');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred while saving the activity: ' + error.message);
    } finally {
        // Loading durumunu kaldır
        submitBtn.disabled = false;
        submitBtn.innerHTML = originalText;
    }
}

function collectFormData() {
    const formData = {
        title: document.getElementById('title').value,
        description: document.getElementById('description').value,
        category: document.getElementById('category').value,
        subcategory: document.getElementById('subcategory').value,
        duration: parseInt(document.getElementById('duration').value),
        rating: parseFloat(document.getElementById('rating')?.value) || 0,
        
        contactInfo: {
            name: document.getElementById('contactName').value,
            email: document.getElementById('contactEmail').value,
            phone: document.getElementById('contactPhone').value,
            role: document.getElementById('contactRole').value
        },
        
        location: {
            address: document.getElementById('address').value,
            city: document.getElementById('city').value,
            country: document.getElementById('country').value,
            latitude: parseFloat(document.getElementById('latitude').value) || 0,
            longitude: parseFloat(document.getElementById('longitude').value) || 0
        },
        
        priceInfo: {
            basePrice: parseFloat(document.querySelector('input[name="priceCategories[0].amount"]')?.value) || 0,
            currency: document.querySelector('select[name="priceCategories[0].currency"]')?.value || 'TRY',
            minimumParticipants: parseInt(document.getElementById('minParticipants')?.value) || 1,
            maximumParticipants: parseInt(document.getElementById('maxParticipants')?.value) || 999
        },
        
        pricing: {
            defaultCurrency: document.querySelector('select[name="priceCategories[0].currency"]')?.value || 'TRY',
            taxRate: parseFloat(document.getElementById('taxRate')?.value) || 0,
            taxIncluded: document.getElementById('taxIncluded')?.checked || false
        },

        // Zaman dilimlerini topla
        timeSlots: [],
        
        // Buluşma noktalarını topla
        meetingPoints: [],
        
        // Dil seçeneklerini topla
        languages: [],
        
        // Dahil/Hariç hizmetleri topla
        included: [],
        excluded: [],
        
        // Gereksinimler
        requirements: [],
        
        // Durum
        status: document.getElementById('status')?.value || 'draft',
        
        // Ek bilgiler
        additionalNotes: document.getElementById('additionalNotes')?.value || '',
        cancellationPolicy: document.getElementById('cancellationPolicy')?.value || ''
    };

    // Zaman dilimlerini topla
    document.querySelectorAll('.time-slot').forEach(function(slot) {
        const daysInputs = slot.querySelectorAll('input[name="days[]"]:checked');
        const days = Array.from(daysInputs).map(input => input.value);
        
        const startTime = slot.querySelector('input[name$=".startTime"]')?.value;
        const endTime = slot.querySelector('input[name$=".endTime"]')?.value;
        
        if (days.length > 0 && startTime && endTime) {
            formData.timeSlots.push({
                startTime: startTime,
                endTime: endTime,
                daysOfWeek: days.join(',')
            });
        }
    });

    // Buluşma noktalarını topla
    document.querySelectorAll('.meeting-point').forEach(function(point) {
        const name = point.querySelector('input[name$=".name"]')?.value;
        const address = point.querySelector('input[name$=".address"]')?.value;
        const latitude = parseFloat(point.querySelector('input[name$=".latitude"]')?.value);
        const longitude = parseFloat(point.querySelector('input[name$=".longitude"]')?.value);
        
        if (name && address) {
            formData.meetingPoints.push({
                name: name,
                address: address,
                latitude: latitude || 0,
                longitude: longitude || 0
            });
        }
    });

    // Dilleri topla
    document.querySelectorAll('select[name="languages[]"]').forEach(function(select) {
        if (select.value) {
            formData.languages.push(select.value);
        }
    });

    // Dahil olan hizmetleri topla
    document.querySelectorAll('#includedServices input[type="text"]').forEach(function(input) {
        if (input.value.trim()) {
            formData.included.push(input.value.trim());
        }
    });

    // Hariç olan hizmetleri topla
    document.querySelectorAll('#excludedServices input[type="text"]').forEach(function(input) {
        if (input.value.trim()) {
            formData.excluded.push(input.value.trim());
        }
    });

    // Gereksinimleri topla
    document.querySelectorAll('#requirements input[type="text"]').forEach(function(input) {
        if (input.value.trim()) {
            formData.requirements.push(input.value.trim());
        }
    });

    console.log('Form data:', formData); // Debug için
    return formData;
}

function initMap() {
    if (map) return;
    
    const mapContainer = document.getElementById('map');
    if (!mapContainer) return;
    
    try {
        map = L.map('map').setView([defaultLocation.lat, defaultLocation.lng], 13);
        
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '© OpenStreetMap contributors'
        }).addTo(map);
        
        marker = L.marker([defaultLocation.lat, defaultLocation.lng], {
            draggable: true
        }).addTo(map);
        
        marker.on('dragend', function(e) {
            const position = marker.getLatLng();
            updateCoordinates(position);
            reverseGeocode(position.lat, position.lng);
        });
        
        initSearch();
        
        setTimeout(() => {
            map.invalidateSize();
        }, 100);
    } catch (error) {
        console.error('Error initializing map:', error);
    }
}

function updateProgressBar() {
    const progress = (currentStep / totalSteps) * 100;
    document.querySelector('.progress-bar').style.width = progress + '%';
    document.querySelector('.progress-bar').textContent = `Step ${currentStep} of ${totalSteps}`;
}

function updateStepIndicators() {
    const steps = document.querySelectorAll('.step');
    steps.forEach(function(step, index) {
        if (index + 1 === currentStep) {
            step.classList.add('active');
        } else {
            step.classList.remove('active');
        }
    });
}

function initSearch() {
    const searchInput = document.getElementById('searchLocation');
    if (!searchInput) return;
    
    searchInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            const query = this.value;
            if (!query.trim()) return;
            
            searchLocation(query);
        }
    });
}

function searchLocation(query) {
    fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(query)}`)
        .then(response => response.json())
        .then(data => {
            if (data && data.length > 0) {
                const location = data[0];
                const latlng = [parseFloat(location.lat), parseFloat(location.lon)];
                map.setView(latlng, 13);
                marker.setLatLng(latlng);
                updateCoordinates(marker.getLatLng());
                reverseGeocode(location.lat, location.lon);
            }
        })
        .catch(error => {
            console.error('Error searching location:', error);
        });
}

function updateCoordinates(position) {
    document.getElementById('latitude').value = position.lat.toFixed(6);
    document.getElementById('longitude').value = position.lng.toFixed(6);
}

function reverseGeocode(lat, lon) {
    fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lon}`)
        .then(response => response.json())
        .then(data => {
            if (data && data.address) {
                document.getElementById('address').value = data.display_name || '';
                document.getElementById('city').value = data.address.city || data.address.town || '';
                document.getElementById('country').value = data.address.country || '';
            }
        })
        .catch(error => {
            console.error('Error reverse geocoding:', error);
        });
}

function updateSubcategories() {
    const categorySelect = document.getElementById('category');
    const subcategorySelect = document.getElementById('subcategory');
    
    // Clear existing options
    subcategorySelect.innerHTML = '';
    
    // Add default option
    const defaultOption = document.createElement('option');
    defaultOption.value = '';
    defaultOption.textContent = 'Select a subcategory';
    subcategorySelect.appendChild(defaultOption);
    
    // Get selected category
    const selectedCategory = categorySelect.value;
    
    // If a category is selected, populate subcategories
    if (selectedCategory && subcategories[selectedCategory]) {
        subcategories[selectedCategory].forEach(sub => {
            const option = document.createElement('option');
            option.value = sub;
            option.textContent = sub;
            subcategorySelect.appendChild(option);
        });
    }
}

function toggleChildTicket(checkbox) {
    const childTicketFields = document.getElementById('childTicketFields');
    const childTicketInputs = document.getElementsByClassName('child-ticket-input');
    
    if (checkbox.checked) {
        childTicketFields.style.display = 'block';
        for (let input of childTicketInputs) {
            input.required = true;
        }
    } else {
        childTicketFields.style.display = 'none';
        for (let input of childTicketInputs) {
            input.required = false;
            input.value = input.type === 'number' ? '' : input.defaultValue;
        }
    }
}

function addPriceCategory() {
    const template = `
        <div class="price-category border rounded p-3 mb-3">
            <div class="row">
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Kategori Tipi</label>
                        <select class="form-select category-type" name="priceCategories[${categoryCounter}].type" required>
                            <option value="">Kategori seçin</option>
                            <option value="Adult">Adult</option>
                            <option value="Child">Child</option>
                            <option value="Infant">Infant</option>
                            <option value="Senior">Senior</option>
                            <option value="Youth">Youth</option>
                            <option value="Student">Student</option>
                            <option value="Military">Military</option>
                            <option value="Group">Group</option>
                            <option value="Traveller">Traveller</option>
                            <option value="Family">Family</option>
                            <option value="Transfer">Transfer</option>
                            <option value="Room">Room</option>
                            <option value="Other">Other</option>
                        </select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Fiyat Tipi</label>
                        <select class="form-select price-type" name="priceCategories[${categoryCounter}].priceType" required>
                            <option value="">Fiyat tipi seçin</option>
                            <option value="Fixed">Sabit</option>
                            <option value="Variable">Değişken</option>
                        </select>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Tutar</label>
                        <input type="number" class="form-control amount" name="priceCategories[${categoryCounter}].amount" min="0" step="0.01" required>
                    </div>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-12">
                    <div class="form-group">
                        <label>Açıklama</label>
                        <textarea class="form-control description" name="priceCategories[${categoryCounter}].description" rows="2"></textarea>
                    </div>
                </div>
            </div>
            <div class="text-end mt-2">
                <button type="button" class="btn btn-danger btn-sm remove-category" onclick="removePriceCategory(this)">
                    <i class="fas fa-trash"></i> Kategoriyi Sil
                </button>
            </div>
        </div>
    `;
    
    document.getElementById('priceCategories').insertAdjacentHTML('beforeend', template);
    categoryCounter++;
}

function removePriceCategory(button) {
    const category = button.closest('.price-category');
    if (document.querySelectorAll('.price-category').length > 1) {
        category.remove();
    } else {
        alert('En az bir fiyat kategorisi olmalıdır.');
    }
}

function addTimeSlot() {
    const template = `
        <div class="time-slot border rounded p-3 mb-3">
            <div class="row">
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Başlangıç Saati</label>
                        <input type="time" class="form-control" name="timeSlots[${timeSlotCounter}].startTime" required>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Bitiş Saati</label>
                        <input type="time" class="form-control" name="timeSlots[${timeSlotCounter}].endTime" required>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Günler</label>
                        <div class="days-checkboxes">
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" name="timeSlots[${timeSlotCounter}].days[]" value="Monday" id="monday-${timeSlotCounter}">
                                <label class="form-check-label" for="monday-${timeSlotCounter}">Pazartesi</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" name="timeSlots[${timeSlotCounter}].days[]" value="Tuesday" id="tuesday-${timeSlotCounter}">
                                <label class="form-check-label" for="tuesday-${timeSlotCounter}">Salı</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" name="timeSlots[${timeSlotCounter}].days[]" value="Wednesday" id="wednesday-${timeSlotCounter}">
                                <label class="form-check-label" for="wednesday-${timeSlotCounter}">Çarşamba</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" name="timeSlots[${timeSlotCounter}].days[]" value="Thursday" id="thursday-${timeSlotCounter}">
                                <label class="form-check-label" for="thursday-${timeSlotCounter}">Perşembe</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" name="timeSlots[${timeSlotCounter}].days[]" value="Friday" id="friday-${timeSlotCounter}">
                                <label class="form-check-label" for="friday-${timeSlotCounter}">Cuma</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" name="timeSlots[${timeSlotCounter}].days[]" value="Saturday" id="saturday-${timeSlotCounter}">
                                <label class="form-check-label" for="saturday-${timeSlotCounter}">Cumartesi</label>
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" name="timeSlots[${timeSlotCounter}].days[]" value="Sunday" id="sunday-${timeSlotCounter}">
                                <label class="form-check-label" for="sunday-${timeSlotCounter}">Pazar</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="text-end mt-2">
                <button type="button" class="btn btn-danger" onclick="removeTimeSlot(this)">
                    <i class="fas fa-trash"></i> Sil
                </button>
            </div>
        </div>
    `;
    
    document.getElementById('timeSlotsContainer').insertAdjacentHTML('beforeend', template);
    timeSlotCounter++;
}

function removeTimeSlot(button) {
    button.closest('.time-slot').remove();
}

function initLocationMap() {
    if (locationMap) return; // Prevent multiple initializations
    
    const mapElement = document.getElementById('locationMap');
    if (!mapElement) return; // Don't initialize if element doesn't exist
    
    locationMap = L.map('locationMap').setView([39.9334, 32.8597], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
    }).addTo(locationMap);

    // Add click event to map
    locationMap.on('click', function(e) {
        const lat = e.latlng.lat;
        const lng = e.latlng.lng;
        
        updateLocationMarker(lat, lng);
        updateLocationFields(lat, lng);
        
        // Get address from coordinates using reverse geocoding
        fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}`)
            .then(response => response.json())
            .then(data => {
                const address = data.display_name;
                document.getElementById('address').value = address;
                
                // Try to extract city and country
                if (data.address) {
                    if (data.address.city) {
                        document.getElementById('city').value = data.address.city;
                    } else if (data.address.town) {
                        document.getElementById('city').value = data.address.town;
                    } else if (data.address.county) {
                        document.getElementById('city').value = data.address.county;
                    }
                    
                    if (data.address.country) {
                        document.getElementById('country').value = data.address.country;
                    }
                }
            })
            .catch(error => {
                console.error('Error reverse geocoding:', error);
            });
    });
}

function searchLocation() {
    const searchInput = document.getElementById('locationSearch');
    const searchQuery = searchInput.value.trim();
    
    if (!searchQuery) {
        alert('Lütfen bir adres girin.');
        return;
    }
    
    if (!locationMap) {
        initLocationMap();
    }

    fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(searchQuery)}`)
        .then(response => response.json())
        .then(data => {
            if (data && data.length > 0) {
                const location = data[0];
                const lat = parseFloat(location.lat);
                const lon = parseFloat(location.lon);
                
                locationMap.setView([lat, lon], 15);
                updateLocationMarker(lat, lon);
                updateLocationFields(lat, lon);
                
                // Update address fields
                document.getElementById('address').value = location.display_name;
                
                // Try to extract city and country
                if (location.address) {
                    if (location.address.city) {
                        document.getElementById('city').value = location.address.city;
                    } else if (location.address.town) {
                        document.getElementById('city').value = location.address.town;
                    } else if (location.address.county) {
                        document.getElementById('city').value = location.address.county;
                    }
                    
                    if (location.address.country) {
                        document.getElementById('country').value = location.address.country;
                    }
                }
            } else {
                alert('Adres bulunamadı.');
            }
        })
        .catch(error => {
            console.error('Error searching location:', error);
            alert('Adres arama sırasında bir hata oluştu.');
        });
}

function updateLocationMarker(lat, lng) {
    // Remove existing marker if any
    if (locationMarker) {
        locationMarker.remove();
    }
    
    // Add new marker
    locationMarker = L.marker([lat, lng]).addTo(locationMap);
}

function updateLocationFields(lat, lng) {
    document.getElementById('latitude').value = lat;
    document.getElementById('longitude').value = lng;
}

function initMeetingPointMap() {
    if (meetingPointMap) return; // Prevent multiple initializations
    
    const mapElement = document.getElementById('meetingPointMap');
    if (!mapElement) return; // Don't initialize if element doesn't exist
    
    meetingPointMap = L.map('meetingPointMap').setView([39.9334, 32.8597], 13);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
    }).addTo(meetingPointMap);

    // Add click event to map
    meetingPointMap.on('click', function(e) {
        const lat = e.latlng.lat;
        const lng = e.latlng.lng;
        
        // Clear existing markers
        meetingPointMarkers.forEach(marker => marker.remove());
        meetingPointMarkers = [];
        
        // Add new marker
        const marker = L.marker([lat, lng]).addTo(meetingPointMap);
        meetingPointMarkers.push(marker);
        
        // Update form fields
        document.getElementById('meetingPointLat').value = lat;
        document.getElementById('meetingPointLng').value = lng;
        
        // Get address from coordinates using reverse geocoding
        fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lng}`)
            .then(response => response.json())
            .then(data => {
                document.getElementById('meetingPointAddress').value = data.display_name;
            })
            .catch(error => {
                console.error('Error reverse geocoding:', error);
            });
    });
}

function showMeetingPointForm() {
    if (!meetingPointModal) {
        meetingPointModal = new bootstrap.Modal(document.getElementById('meetingPointModal'));
    }
    
    // Clear form
    document.getElementById('meetingPointName').value = '';
    document.getElementById('meetingPointAddress').value = '';
    document.getElementById('meetingPointLat').value = '';
    document.getElementById('meetingPointLng').value = '';
    
    // Show modal
    meetingPointModal.show();
    
    // Initialize map after modal is shown
    setTimeout(() => {
        if (!meetingPointMap) {
            initMeetingPointMap();
        }
        meetingPointMap.invalidateSize();
    }, 500);
}

function searchMeetingPoint() {
    const searchInput = document.getElementById('meetingPointSearch');
    const searchQuery = searchInput.value.trim();
    
    if (!searchQuery) {
        alert('Lütfen bir adres girin.');
        return;
    }

    fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(searchQuery)}`)
        .then(response => response.json())
        .then(data => {
            if (data && data.length > 0) {
                const location = data[0];
                const lat = parseFloat(location.lat);
                const lon = parseFloat(location.lon);
                
                if (meetingPointMap) {
                    meetingPointMap.setView([lat, lon], 15);
                    
                    // Clear existing markers
                    meetingPointMarkers.forEach(marker => marker.remove());
                    meetingPointMarkers = [];
                    
                    // Add new marker
                    const marker = L.marker([lat, lon]).addTo(meetingPointMap);
                    meetingPointMarkers.push(marker);
                    
                    // Update form fields
                    document.getElementById('meetingPointAddress').value = location.display_name;
                    document.getElementById('meetingPointLat').value = lat;
                    document.getElementById('meetingPointLng').value = lon;
                }
            } else {
                alert('Adres bulunamadı.');
            }
        })
        .catch(error => {
            console.error('Error searching location:', error);
            alert('Adres arama sırasında bir hata oluştu.');
        });
}

function saveMeetingPoint() {
    const name = document.getElementById('meetingPointName').value.trim();
    const address = document.getElementById('meetingPointAddress').value.trim();
    const lat = document.getElementById('meetingPointLat').value;
    const lng = document.getElementById('meetingPointLng').value;
    
    if (!name || !address || !lat || !lng) {
        alert('Lütfen tüm alanları doldurun ve haritadan bir konum seçin.');
        return;
    }
    
    const template = `
        <div class="meeting-point border rounded p-3 mb-3">
            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label>Buluşma Noktası Adı</label>
                        <input type="text" class="form-control" name="meetingPoints[${meetingPointCounter}].name" value="${name}" required readonly>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label>Buluşma Noktası Adresi</label>
                        <input type="text" class="form-control" name="meetingPoints[${meetingPointCounter}].address" value="${address}" required readonly>
                    </div>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-6">
                    <div class="form-group">
                        <label>Enlem</label>
                        <input type="number" class="form-control" name="meetingPoints[${meetingPointCounter}].latitude" value="${lat}" step="any" required readonly>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <label>Boylam</label>
                        <input type="number" class="form-control" name="meetingPoints[${meetingPointCounter}].longitude" value="${lng}" step="any" required readonly>
                    </div>
                </div>
            </div>
            <div class="text-end mt-2">
                <button type="button" class="btn btn-danger" onclick="removeMeetingPoint(this, ${meetingPointCounter})">
                    <i class="fas fa-trash"></i> Sil
                </button>
            </div>
        </div>
    `;
    
    document.getElementById('meetingPointsContainer').insertAdjacentHTML('beforeend', template);
    meetingPointCounter++;
    
    // Close modal
    meetingPointModal.hide();
}

function removeMeetingPoint(button, index) {
    // Remove marker from map
    if (meetingPointMarkers[index]) {
        meetingPointMap.removeLayer(meetingPointMarkers[index]);
        meetingPointMarkers[index] = null;
    }
    
    // Remove form element
    button.closest('.meeting-point').remove();
}

function addLanguage() {
    const languagesDiv = document.getElementById('languages');
    const template = `
        <div class="input-group mb-2">
            <select class="form-control language-select" name="languages[]" required>
                <option value="">Select a language</option>
                <option value="tr">Türkçe</option>
                <option value="en">English</option>
                <option value="de">Deutsch</option>
                <option value="fr">Français</option>
                <option value="es">Español</option>
                <option value="it">Italiano</option>
                <option value="ru">Русский</option>
                <option value="ar">العربية</option>
            </select>
            <button type="button" class="btn btn-danger remove-language" onclick="removeLanguage(this)">Remove</button>
        </div>
    `;
    languagesDiv.insertAdjacentHTML('beforeend', template);
}

function removeLanguage(button) {
    const languageGroup = button.closest('.input-group');
    if (document.querySelectorAll('.language-select').length > 1) {
        languageGroup.remove();
    } else {
        alert('At least one language must be selected.');
    }
}

// Initialize maps when steps are shown
document.addEventListener('DOMContentLoaded', function() {
    const observer = new MutationObserver(function(mutations) {
        mutations.forEach(function(mutation) {
            if (mutation.target.style.display !== 'none') {
                if (mutation.target.id === 'step2') {
                    setTimeout(() => {
                        initLocationMap();
                    }, 100);
                } else if (mutation.target.id === 'step5') {
                    setTimeout(() => {
                        initMeetingPointMap();
                    }, 100);
                }
            }
        });
    });

    // Observe both step2 and step5
    const step2 = document.getElementById('step2');
    const step5 = document.getElementById('step5');
    
    if (step2) {
        observer.observe(step2, {
            attributes: true,
            attributeFilter: ['style']
        });
    }
    
    if (step5) {
        observer.observe(step5, {
            attributes: true,
            attributeFilter: ['style']
        });
    }
});

function updatePreview() {
    const formData = collectFormData();
    
    // Basic Information
    document.getElementById('preview-title').textContent = formData.title || '-';
    document.getElementById('preview-category').textContent = formData.category || '-';
    document.getElementById('preview-subcategory').textContent = formData.subcategory || '-';
    document.getElementById('preview-duration').textContent = formData.duration ? `${formData.duration} dakika` : '-';
    document.getElementById('preview-description').textContent = formData.description || '-';
    document.getElementById('preview-status').textContent = formData.status || '-';
    document.getElementById('preview-languages').textContent = formData.languages.join(', ') || '-';
    
    // Location & Contact
    document.getElementById('preview-address').textContent = formData.location.address || '-';
    document.getElementById('preview-city').textContent = formData.location.city || '-';
    document.getElementById('preview-country').textContent = formData.location.country || '-';
    document.getElementById('preview-contact-name').textContent = formData.contactInfo.name || '-';
    document.getElementById('preview-contact-email').textContent = formData.contactInfo.email || '-';
    document.getElementById('preview-contact-phone').textContent = formData.contactInfo.phone || '-';
    
    // Pricing
    document.getElementById('preview-base-price').textContent = formData.priceInfo.basePrice ? `${formData.priceInfo.basePrice} ${formData.priceInfo.currency}` : '-';
    document.getElementById('preview-currency').textContent = formData.priceInfo.currency || '-';
    document.getElementById('preview-tax-rate').textContent = formData.pricing.taxRate ? `${formData.pricing.taxRate}` : '-';
    document.getElementById('preview-tax-included').textContent = formData.pricing.taxIncluded ? 'Evet' : 'Hayır';
    document.getElementById('preview-min-participants').textContent = formData.priceInfo.minimumParticipants || '-';
    document.getElementById('preview-max-participants').textContent = formData.priceInfo.maximumParticipants || '-';
    
    // Services & Requirements
    const includedList = document.getElementById('preview-included-services');
    includedList.textContent = formData.included.join(', ') || '-';

    const excludedList = document.getElementById('preview-excluded-services');
    excludedList.textContent = formData.excluded.join(', ') || '-';

    const requirementsList = document.getElementById('preview-requirements');
    requirementsList.textContent = formData.requirements.join(', ') || '-';
    
    // Additional Information
    document.getElementById('preview-cancellation-policy').textContent = formData.cancellationPolicy || '-';
    document.getElementById('preview-additional-notes').textContent = formData.additionalNotes || '-';

    // Time Slots
    const timeSlotsPreview = formData.timeSlots.map(slot => 
        `${slot.daysOfWeek}: ${slot.startTime} - ${slot.endTime}`
    ).join('<br>');
    document.getElementById('preview-time-slots').innerHTML = timeSlotsPreview || '-';
} 