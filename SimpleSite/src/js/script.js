// Medical Chat Tutorial Application
// Optimized class-based implementation with performance enhancements

class TutorialApp {
  constructor() {
    // Application state
    this.tutorialData = null;
    this.slides = [];
    this.currentSlide = 0;
    
    // User progress tracking
    this.userProgress = {
      startTime: null,
      slideTimeSpent: [],
      completedSlides: new Set(),
      userResponses: {},
      currentSlideStartTime: null
    };
    
    // Performance optimizations - DOM element cache
    this.domCache = new Map();
    this.isModalOpen = false;
      // Event listeners with proper binding
    this.boundEventHandlers = {
      resize: this.debounce(this.handleResize.bind(this), 250),
      keydown: this.handleKeydown.bind(this)
    };
    
    // Initialize the application
    this.init();
  }

  // Application initialization
  async init() {
    try {
      await this.loadTutorialData();
      this.initializeDOMCache();
      this.setupEventListeners();
      this.initializeProgressTracking();
      console.log('Tutorial application initialized successfully');
    } catch (error) {
      console.error('Failed to initialize tutorial application:', error);
      this.handleCriticalError(error);
    }
  }
  // DOM cache initialization for performance
  initializeDOMCache() {
    const selectors = {
      tutorialModal: '#tutorialModal',
      slideContainer: '#slideContainer',
      currentCounter: '#current',
      totalCounter: '#total',
      title: 'h1',
      description: 'p',
      startButton: '.btn',
      closeButton: '.btn-close'
    };

    for (const [key, selector] of Object.entries(selectors)) {
      const element = document.querySelector(selector);
      if (element) {
        this.domCache.set(key, element);
      } else {
        console.warn(`Element not found: ${selector}`);
      }
    }
    
    // Initialize Bootstrap modal instance
    const modalElement = this.domCache.get('tutorialModal');
    if (modalElement && typeof bootstrap !== 'undefined') {
      this.bootstrapModal = new bootstrap.Modal(modalElement);
    }
  }  // Efficient event listener setup
  setupEventListeners() {
    window.addEventListener('resize', this.boundEventHandlers.resize, { passive: true });
    document.addEventListener('keydown', this.boundEventHandlers.keydown);
    
    // Bootstrap modal event listeners
    const modalElement = this.domCache.get('tutorialModal');
    if (modalElement) {
      modalElement.addEventListener('shown.bs.modal', () => {
        this.isModalOpen = true;
        this.showSlide(0);
        this.updateSlideCounter();
      });
      
      modalElement.addEventListener('hidden.bs.modal', () => {
        this.isModalOpen = false;
        this.saveProgress();
      });
    }
    
    // Start button
    const startButton = this.domCache.get('startButton');
    if (startButton) {
      startButton.addEventListener('click', () => this.openModal());
    }
    
    // Tutorial selector dropdown
    const tutorialSelect = document.getElementById('tutorialSelect');
    if (tutorialSelect) {
      tutorialSelect.addEventListener('change', (event) => {
        const selectedFile = event.target.value;
        console.log(`Tutorial type changed to: ${selectedFile}`);
        // Clear cached data for dynamic loading
        this.clearCache();
      });
    }
  }
  // Enhanced data loading with error handling and caching
  async loadTutorialData(dataFile = 'data/slides.json') {
    try {
      // Check for cached data first
      const cacheKey = `tutorialData_${dataFile}`;
      const cachedData = this.getCachedTutorialData(cacheKey);
      if (cachedData) {
        console.log(`Using cached tutorial data for ${dataFile}`);
        this.processTutorialData(cachedData);
        return;
      }      // Load data with timeout and abort controller
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 10000);

      const response = await fetch(dataFile, {
        signal: controller.signal,
        cache: 'default', // Allow browser caching
        headers: {
          'Accept': 'application/json',
          'Cache-Control': 'max-age=300' // 5 minutes
        }
      });
      
      clearTimeout(timeoutId);

      if (!response.ok) {
        throw new Error(`HTTP ${response.status}: ${response.statusText}`);
      }

      const data = await response.json();
      this.processTutorialData(data);
      this.cacheTutorialData(data, cacheKey);
      
    } catch (error) {
      console.error('Error loading tutorial data:', error);
      
      if (error.name === 'AbortError') {
        throw new Error('Request timed out. Please check your connection.');
      }
      
      this.handleTutorialLoadError(error);
    }
  }

  // Process and validate tutorial data
  processTutorialData(data) {
    this.validateTutorialData(data);
    this.tutorialData = data;
    this.slides = data.slides;
    this.updatePageMetadata();
    this.setupNavigation();
  }

  // Comprehensive data validation
  validateTutorialData(data) {
    if (!data || typeof data !== 'object') {
      throw new Error('Invalid data format: Expected object');
    }
    
    if (!data.tutorial) {
      throw new Error('Missing tutorial configuration');
    }
    
    if (!Array.isArray(data.slides) || data.slides.length === 0) {
      throw new Error('Invalid slides: Expected non-empty array');
    }

    // Validate each slide has required properties
    data.slides.forEach((slide, index) => {
      if (!slide.id || !slide.title) {
        throw new Error(`Invalid slide ${index}: Missing required properties`);
      }
    });

    // Update slide count to match actual slides
    data.tutorial.slideCount = data.slides.length;
  }
  // Cache management for performance
  getCachedTutorialData(cacheKey = 'tutorialData') {
    try {
      const cached = localStorage.getItem(cacheKey);
      const timestamp = localStorage.getItem(`${cacheKey}Timestamp`);
      
      if (cached && timestamp) {
        const age = Date.now() - parseInt(timestamp);
        // Cache valid for 1 hour
        if (age < 3600000) {
          return JSON.parse(cached);
        }
      }
    } catch (error) {
      console.warn('Error reading cached data:', error);
    }
    return null;
  }

  cacheTutorialData(data, cacheKey = 'tutorialData') {
    try {
      localStorage.setItem(cacheKey, JSON.stringify(data));
      localStorage.setItem(`${cacheKey}Timestamp`, Date.now().toString());
    } catch (error) {
      console.warn('Error caching data:', error);
    }
  }

  // Clear all cached tutorial data
  clearCache() {
    try {
      const keys = Object.keys(localStorage);
      const tutorialKeys = keys.filter(key => 
        key.startsWith('tutorialData') || key.includes('tutorialData')
      );
      
      tutorialKeys.forEach(key => localStorage.removeItem(key));
      console.log('Tutorial cache cleared');
    } catch (error) {
      console.warn('Error clearing cache:', error);
    }
  }

  // Enhanced error handling with fallback content
  handleTutorialLoadError(error) {
    const fallbackData = {
      tutorial: {
        title: "Medical Chat Tutorial",
        description: "Learn how to use our mobile medical consultation platform",
        slideCount: 1,
        navigation: {
          previousText: "← Back",
          nextText: "Next →",
          closeText: "×",
          startButtonText: "Start Tutorial",
          previousLocation: "bottom",
          nextLocation: "bottom"
        },
        settings: {
          showCounter: true,
          allowKeyboardNavigation: true,
          imageSize: { width: 200, height: 300 }
        }
      },
      slides: [
        {
          id: 1,
          title: "Welcome to MedChat",
          text: "Tutorial content is temporarily unavailable. Please try again later.",
          media: { 
            img: "https://placehold.co/200x300/4A90E2/FFF?text=Welcome+to+MedChat",
            altText: "Welcome screen placeholder"
          }
        }
      ]
    };
    
    this.processTutorialData(fallbackData);
    this.showNotification(`Tutorial loaded with limited functionality: ${error.message}`, 'warning');
  }  // Critical error handler
  handleCriticalError(error) {
    const errorContainer = document.createElement('div');
    errorContainer.className = 'position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center';
    errorContainer.style.cssText = 'background: rgba(0, 0, 0, 0.9); z-index: 3000;';
    errorContainer.innerHTML = `
      <div class="card text-center" style="max-width: 500px; margin: 20px;">
        <div class="card-body p-5">
          <i class="bi bi-exclamation-triangle text-danger" style="font-size: 3rem;"></i>
          <h2 class="card-title text-danger mt-3">Application Error</h2>
          <p class="card-text">The tutorial application failed to load properly.</p>
          <p class="text-muted small">${error.message}</p>
          <button onclick="location.reload()" class="btn btn-primary mt-3">
            <i class="bi bi-arrow-clockwise me-2"></i>Reload Page
          </button>
        </div>
      </div>
    `;
    document.body.appendChild(errorContainer);
  }  // User-friendly notification system
  showNotification(message, type = 'info', duration = 5000) {
    const notification = document.createElement('div');
    notification.className = `position-fixed alert alert-${this.getBootstrapAlertType(type)} alert-dismissible fade show notification-slide`;
    notification.style.cssText = 'top: 1.25rem; right: 1.25rem; z-index: 2000; max-width: 25rem;';
    
    const icons = { info: 'bi-info-circle', warning: 'bi-exclamation-triangle', error: 'bi-x-circle', success: 'bi-check-circle' };
    notification.innerHTML = `
      <div class="d-flex align-items-center">
        <i class="${icons[type] || icons.info} me-2"></i>
        <span class="flex-grow-1">${message}</span>
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
      </div>
    `;
    
    document.body.appendChild(notification);
    
    // Auto-remove with animation
    setTimeout(() => {
      if (notification.parentNode) {
        notification.classList.remove('show');
        setTimeout(() => notification.remove(), 300);
      }
    }, duration);
  }

  getBootstrapAlertType(type) {
    const typeMap = {
      info: 'primary',
      warning: 'warning', 
      error: 'danger',
      success: 'success'
    };
    return typeMap[type] || 'primary';
  }

  // Progress tracking initialization
  initializeProgressTracking() {
    if (this.tutorialData?.tutorial.tracking?.saveProgress) {
      this.userProgress.startTime = new Date();
      this.userProgress.slideTimeSpent = new Array(this.slides.length).fill(0);
    }
  }

  // Update page metadata efficiently
  updatePageMetadata() {
    const tutorial = this.tutorialData.tutorial;
    
    // Batch DOM updates
    requestAnimationFrame(() => {
      const title = this.domCache.get('title');
      const description = this.domCache.get('description');
      const startButton = this.domCache.get('startButton');
      const closeButton = this.domCache.get('closeButton');
      
      if (title) title.textContent = tutorial.title;
      if (description) description.textContent = tutorial.description;
      if (startButton) startButton.textContent = tutorial.navigation.startButtonText;
      if (closeButton) closeButton.textContent = tutorial.navigation.closeText;
    });
  }
  // Dynamic navigation setup
  setupNavigation() {
    const nav = this.tutorialData.tutorial.navigation;
    const modalBody = document.querySelector('.modal-body');
    if (!modalBody) return;
    
    // Remove existing navigation
    this.clearExistingNavigation();
    
    // Create navigation elements using Bootstrap classes
    this.createNavigationElements(nav, modalBody);
  }

  clearExistingNavigation() {
    const existingNavs = document.querySelectorAll('.tutorial-nav, .tutorial-top-nav');
    existingNavs.forEach(nav => nav.remove());
  }

  createNavigationElements(nav, modalBody) {
    const slideContainer = this.domCache.get('slideContainer');
    
    // Top navigation
    if (nav.previousLocation === 'top' || nav.nextLocation === 'top') {
      const topNav = this.createNavigation('tutorial-top-nav', nav, 'top');
      modalBody.insertBefore(topNav, slideContainer);
    }
    
    // Bottom navigation
    if (nav.previousLocation === 'bottom' || nav.nextLocation === 'bottom') {
      const bottomNav = this.createNavigation('tutorial-nav', nav, 'bottom');
      modalBody.appendChild(bottomNav);
    }
  }

  createNavigation(className, nav, location) {
    const navElement = document.createElement('div');
    navElement.className = `${className} d-flex justify-content-between align-items-center mt-3`;
    
    const buttons = [];
    
    // Previous button
    if (nav.previousLocation === location) {
      buttons.push(`<button class="btn btn-outline-secondary" id="prevBtn">${nav.previousText}</button>`);
    } else if (location === 'bottom') {
      buttons.push('<div></div>');
    }
    
    // Counter (only in bottom navigation)
    if (location === 'bottom') {
      buttons.push('<span class="text-muted"><span id="current">1</span> / <span id="total">5</span></span>');
    }
    
    // Next button
    if (nav.nextLocation === location) {
      buttons.push(`<button class="btn btn-primary" id="nextBtn">${nav.nextText}</button>`);
    } else if (location === 'bottom') {
      buttons.push('<div></div>');
    }
    
    navElement.innerHTML = buttons.join('');
    
    // Add event listeners to buttons
    this.addNavigationEventListeners(navElement);
    
    return navElement;
  }

  addNavigationEventListeners(navElement) {
    const prevBtn = navElement.querySelector('#prevBtn');
    const nextBtn = navElement.querySelector('#nextBtn');
    
    if (prevBtn) {
      prevBtn.addEventListener('click', () => this.previousSlide());
    }
    
    if (nextBtn) {
      nextBtn.addEventListener('click', () => this.nextSlide());
    }
  }  // Modal operations
  async openModal() {
    // Get selected tutorial type
    const tutorialSelect = document.getElementById('tutorialSelect');
    const selectedDataFile = tutorialSelect ? tutorialSelect.value : 'data/slides.json';
    
    try {
      // Load the selected tutorial data
      await this.loadTutorialData(selectedDataFile);
      
      if (this.bootstrapModal) {
        this.bootstrapModal.show();
      } else {
        // Fallback for cases where Bootstrap is not available
        const modal = this.domCache.get('tutorialModal');
        if (modal) {
          modal.style.display = "block";
          this.isModalOpen = true;
          this.showSlide(0);
          this.updateSlideCounter();
        }
      }
      
      // Update total counter
      const totalCounter = document.getElementById('total');
      if (totalCounter) {
        totalCounter.textContent = this.tutorialData.tutorial.slideCount;
      }
    } catch (error) {
      console.error('Failed to open modal:', error);
      this.showNotification('Failed to load tutorial. Please try again.', 'error');
    }
  }

  closeModal() {
    if (this.bootstrapModal) {
      this.bootstrapModal.hide();
    } else {
      // Fallback for cases where Bootstrap is not available
      const modal = this.domCache.get('tutorialModal');
      if (modal) {
        modal.style.display = "none";
        this.isModalOpen = false;
        this.saveProgress();
      }
    }
  }

  // Optimized slide display with batched DOM updates
  showSlide(index) {
    if (index < 0 || index >= this.slides.length) return;
    
    const slideStartTime = performance.now();
    
    // Track time spent on previous slide
    this.trackSlideTime();
    
    this.currentSlide = index;
    const slide = this.slides[index];
    const settings = this.tutorialData.tutorial.settings;
    
    // Build slide content efficiently
    const slideHTML = this.buildSlideHTML(slide, settings);
    
    // Batch DOM updates
    requestAnimationFrame(() => {
      const container = this.domCache.get('slideContainer');
      if (container) {
        container.innerHTML = slideHTML;
        
        // Initialize interactive elements after DOM update
        this.initializeSlideInteractions(slide);
        
        // Update navigation state
        this.updateNavigationState();
        
        // Update counter
        this.updateSlideCounter();
        
        // Mark slide as completed
        this.userProgress.completedSlides.add(index);
        
        // Set current slide start time
        this.userProgress.currentSlideStartTime = slideStartTime;
        
        // Handle completion on last slide
        if (index === this.slides.length - 1 && slide.completion?.showCertificate) {
          setTimeout(() => this.showCompletionCertificate(slide), 1000);
        }
      }
    });
  }
  buildSlideHTML(slide, settings) {
    const components = [];
    
    // Use Bootstrap classes for layout
    components.push(`<div class="slide active">`);
    components.push(`<h2 class="h3 text-primary mb-3">${this.escapeHTML(slide.title)}</h2>`);
    
    if (slide.subtitle) {
      components.push(`<h3 class="h5 text-muted mb-3">${this.escapeHTML(slide.subtitle)}</h3>`);
    }
    
    // Media content
    if (slide.media || slide.img) {
      components.push(this.buildMediaHTML(slide, settings));
    }
    
    // Main text
    components.push(`<p class="lead mb-3">${this.escapeHTML(slide.text)}</p>`);
    
    // Bullet points
    if (slide.bulletPoints?.length > 0) {
      components.push(this.buildBulletPointsHTML(slide.bulletPoints));
    }
    
    // Tips and warnings
    if (slide.tip) {
      components.push(`<div class="alert alert-success d-flex align-items-center" role="alert">
        <i class="bi bi-lightbulb me-2"></i>
        <div>${this.escapeHTML(slide.tip)}</div>
      </div>`);
    }
    
    if (slide.warning) {
      components.push(`<div class="alert alert-warning d-flex align-items-center" role="alert">
        <i class="bi bi-exclamation-triangle me-2"></i>
        <div>${this.escapeHTML(slide.warning)}</div>
      </div>`);
    }
    
    // Interactive content
    if (slide.interactive) {
      components.push(this.buildInteractiveHTML(slide.interactive, slide.id));
    }
    
    components.push(`</div>`);
    
    return components.join('');
  }  buildMediaHTML(slide, settings) {
    const media = slide.media || { img: slide.img };
    const imgWidth = settings.imageSize?.width || 200;
    const imgHeight = settings.imageSize?.height || 300;
    
    return `
      <div class="text-center mb-4">
        <img src="${this.escapeHTML(media.img)}" 
             alt="${this.escapeHTML(media.altText || slide.title)}"
             style="width: ${imgWidth}px; height: ${imgHeight}px;"
             class="img-fluid rounded shadow-sm"
             loading="lazy">
        ${media.caption ? `<div class="text-muted mt-2 small fst-italic">${this.escapeHTML(media.caption)}</div>` : ''}
      </div>
    `;
  }

  buildBulletPointsHTML(bulletPoints) {
    const listItems = bulletPoints.map(point => `<li class="mb-2">${this.escapeHTML(point)}</li>`).join('');
    return `<ul class="list-unstyled ps-3 mb-4">${listItems}</ul>`;
  }

  buildInteractiveHTML(interactive, slideId) {
    switch (interactive.type) {
      case 'quiz':
        return this.buildQuizHTML(interactive, slideId);
      case 'checklist':
        return this.buildChecklistHTML(interactive, slideId);
      default:
        return '';
    }
  }
  buildQuizHTML(interactive, slideId) {
    const options = interactive.options.map((option, index) => `
      <button class="btn btn-outline-primary w-100 text-start quiz-option mb-2" 
              data-slide-id="${slideId}" 
              data-option="${index}" 
              data-correct="${interactive.correctAnswer}">
        ${this.escapeHTML(option)}
      </button>
    `).join('');
    
    return `
      <div class="card border-primary mb-4" id="quiz-${slideId}">
        <div class="card-body">
          <h4 class="card-title h6 text-primary">${this.escapeHTML(interactive.question)}</h4>
          <div class="d-grid gap-2 mt-3">${options}</div>
          <div class="alert alert-info mt-3 d-none" id="quiz-feedback-${slideId}" role="alert"></div>
        </div>
      </div>
    `;
  }

  buildChecklistHTML(interactive, slideId) {
    const items = interactive.items.map((item, index) => `
      <div class="form-check mb-2">
        <input type="checkbox" class="form-check-input" data-slide-id="${slideId}" id="check-${slideId}-${index}">
        <label class="form-check-label" for="check-${slideId}-${index}">
          ${this.escapeHTML(item)}
        </label>
      </div>
    `).join('');
    
    return `
      <div class="card border-success mb-4" id="checklist-${slideId}">
        <div class="card-body">
          <h4 class="card-title h6 text-success">Before continuing:</h4>
          <div class="mt-3">${items}</div>
        </div>
      </div>
    `;
  }
  // Initialize interactive elements after slide render
  initializeSlideInteractions(slide) {
    // Quiz interactions
    const quizOptions = document.querySelectorAll('.quiz-option');
    quizOptions.forEach(option => {
      option.addEventListener('click', (e) => this.handleQuizAnswer(e.target));
    });
    
    // Checklist interactions
    const checkboxes = document.querySelectorAll('.form-check-input[data-slide-id]');
    checkboxes.forEach(checkbox => {
      checkbox.addEventListener('change', (e) => this.handleChecklistChange(e.target));
    });
    
    // Hotspots
    if (slide.hotspots) {
      this.addHotspots(slide.hotspots);
    }
  }
  // Interactive element handlers
  handleQuizAnswer(optionElement) {
    const slideId = parseInt(optionElement.dataset.slideId);
    const selectedAnswer = parseInt(optionElement.dataset.option);
    const correctAnswer = parseInt(optionElement.dataset.correct);
    
    // Disable all options
    const allOptions = document.querySelectorAll(`[data-slide-id="${slideId}"]`);
    allOptions.forEach(option => option.disabled = true);
    
    // Apply Bootstrap visual feedback
    if (selectedAnswer === correctAnswer) {
      optionElement.classList.remove('btn-outline-primary');
      optionElement.classList.add('btn-success');
    } else {
      optionElement.classList.remove('btn-outline-primary');
      optionElement.classList.add('btn-danger');
      // Highlight correct answer
      allOptions[correctAnswer].classList.remove('btn-outline-primary');
      allOptions[correctAnswer].classList.add('btn-success');
    }
    
    // Show feedback
    this.showQuizFeedback(slideId, selectedAnswer === correctAnswer);
    
    // Store response
    this.userProgress.userResponses[`quiz-${slideId}`] = {
      selected: selectedAnswer,
      correct: correctAnswer,
      isCorrect: selectedAnswer === correctAnswer
    };
  }

  showQuizFeedback(slideId, isCorrect) {
    const feedbackEl = document.getElementById(`quiz-feedback-${slideId}`);
    if (!feedbackEl) return;
    
    const slide = this.slides.find(s => s.id === slideId);
    const explanation = slide?.interactive?.explanation || '';
    
    feedbackEl.classList.remove('d-none');
    feedbackEl.className = `alert mt-3 ${isCorrect ? 'alert-success' : 'alert-warning'}`;
    feedbackEl.innerHTML = isCorrect ? 
      `<i class="bi bi-check-circle me-2"></i>Correct! ${explanation}` :
      `<i class="bi bi-x-circle me-2"></i>Not quite right. ${explanation}`;
  }

  handleChecklistChange(checkboxElement) {
    const slideId = parseInt(checkboxElement.dataset.slideId);
    const checkboxes = document.querySelectorAll(`input[data-slide-id="${slideId}"]`);
    const checkedCount = Array.from(checkboxes).filter(cb => cb.checked).length;
    
    this.userProgress.userResponses[`checklist-${slideId}`] = {
      completed: checkedCount,
      total: checkboxes.length,
      percentage: Math.round((checkedCount / checkboxes.length) * 100)
    };
  }  // Hotspot functionality
  addHotspots(hotspots) {
    const slideElement = document.querySelector('.slide');
    if (!slideElement) return;
    
    hotspots.forEach(hotspot => {
      const hotspotEl = document.createElement('div');
      hotspotEl.className = 'position-absolute rounded-circle border border-white border-3 shadow hotspot-pulse';
      hotspotEl.style.cssText = `
        width: 1.5rem;
        height: 1.5rem;
        background: #007bff;
        cursor: pointer;
        z-index: 10;
        left: ${hotspot.x}%;
        top: ${hotspot.y}%;
      `;
      hotspotEl.title = hotspot.tooltip;
      hotspotEl.addEventListener('click', () => {
        if (hotspot.action === 'highlight') {
          hotspotEl.style.background = '#28a745';
          hotspotEl.classList.remove('hotspot-pulse');
          hotspotEl.style.transform = 'scale(1.2)';
          hotspotEl.classList.add('highlighted');
        }
      });
      
      slideElement.appendChild(hotspotEl);
    });
  }

  // Navigation methods
  nextSlide() {
    if (this.currentSlide < this.slides.length - 1) {
      this.showSlide(this.currentSlide + 1);
    }
  }

  previousSlide() {
    if (this.currentSlide > 0) {
      this.showSlide(this.currentSlide - 1);
    }
  }

  // Update navigation button states
  updateNavigationState() {
    const prevButtons = document.querySelectorAll('#prevBtn');
    const nextButtons = document.querySelectorAll('#nextBtn');
    
    prevButtons.forEach(btn => btn.disabled = this.currentSlide === 0);
    nextButtons.forEach(btn => btn.disabled = this.currentSlide === this.slides.length - 1);
  }

  // Update slide counter
  updateSlideCounter() {
    if (this.tutorialData.tutorial.settings.showCounter) {
      const currentCounter = this.domCache.get('currentCounter');
      if (currentCounter) {
        currentCounter.textContent = this.currentSlide + 1;
      }
    }
  }

  // Time tracking
  trackSlideTime() {
    if (this.tutorialData?.tutorial.tracking?.timeSpentTracking && 
        this.userProgress.currentSlideStartTime) {
      const timeSpent = performance.now() - this.userProgress.currentSlideStartTime;
      if (this.userProgress.slideTimeSpent[this.currentSlide] !== undefined) {
        this.userProgress.slideTimeSpent[this.currentSlide] += timeSpent;
      }
    }
  }

  // Completion certificate
  showCompletionCertificate(slide) {
    const certificateText = slide.completion.certificateText || 
                          this.tutorialData.tutorial.tracking?.completionCertificate?.template ||
                          'Congratulations! You have completed the tutorial.';
    
    this.showNotification(`🎉 ${certificateText}`, 'success', 8000);
    
    if (slide.completion.nextSteps) {
      console.log('Next steps:', slide.completion.nextSteps);
    }
  }

  // Progress saving
  saveProgress() {
    if (this.tutorialData?.tutorial.tracking?.saveProgress) {
      try {
        const progressData = {
          ...this.userProgress,
          completedSlides: Array.from(this.userProgress.completedSlides),
          timestamp: Date.now()
        };
        localStorage.setItem('tutorialProgress', JSON.stringify(progressData));
      } catch (error) {
        console.warn('Failed to save progress:', error);
      }
    }
  }
  // Event handlers
  handleResize() {
    // Bootstrap modal handles responsive design automatically
    // This is kept for any custom responsive logic if needed
  }

  handleKeydown(event) {
    if (this.isModalOpen && this.tutorialData?.tutorial.settings.allowKeyboardNavigation) {
      switch (event.key) {
        case 'Escape':
          this.closeModal();
          break;
        case 'ArrowLeft':
          this.previousSlide();
          break;
        case 'ArrowRight':
          this.nextSlide();
          break;
      }
    }
  }

  // Utility methods
  debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
      const later = () => {
        clearTimeout(timeout);
        func.apply(this, args);
      };
      clearTimeout(timeout);
      timeout = setTimeout(later, wait);
    };
  }

  escapeHTML(text) {
    if (typeof text !== 'string') return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  }

  // Cleanup method for memory management
  destroy() {
    // Remove event listeners
    window.removeEventListener('resize', this.boundEventHandlers.resize);
    document.removeEventListener('keydown', this.boundEventHandlers.keydown);
    window.removeEventListener('click', this.boundEventHandlers.click);
    
    // Clear caches
    this.domCache.clear();
    
    // Save final progress
    this.saveProgress();
    
    console.log('Tutorial application destroyed');
  }
}

// Initialize application when DOM is ready
let tutorialApp;

document.addEventListener('DOMContentLoaded', () => {
  tutorialApp = new TutorialApp();
});

// Cleanup on page unload
window.addEventListener('beforeunload', () => {
  if (tutorialApp) {
    tutorialApp.destroy();
  }
});

// Global functions for backward compatibility (if needed)
function openModal() {
  if (tutorialApp) tutorialApp.openModal();
}

function closeModal() {
  if (tutorialApp) tutorialApp.closeModal();
}

function nextSlide() {
  if (tutorialApp) tutorialApp.nextSlide();
}

function prevSlide() {
  if (tutorialApp) tutorialApp.previousSlide();
}
