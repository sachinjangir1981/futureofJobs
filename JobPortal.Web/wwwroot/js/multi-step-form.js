 
    document.addEventListener('DOMContentLoaded', () => {
        const formSteps = document.querySelectorAll('.form-step');
        const formStepContainer = document.getElementById('form-step-container');
        const progressBar = document.getElementById('progress-bar');
        const stepIndicator = document.getElementById('step-indicator');
        const prevButton = document.getElementById('prev-button');
        const nextButton = document.getElementById('next-button');
        const submitButton = document.getElementById('submit-button');
        const userIdforbtn = document.getElementById('UserId');
        const reviewbutton = document.getElementById('review-button');
        const closePopup = document.getElementById('closePopup');
        const popup = document.getElementById('myPopup');
        const overlay = document.getElementById('overlay');


        let currentStep = 0; // 0-indexed for array, 1-indexed for display
        const totalSteps = formSteps.length;

        const formData = {
            name: '',
        email: '',
        address1: '',
        city: '',
        zip: '',
        favoriteColor: '',
        newsletter: false,
                };

    // Function to update the form display based on currentStep
    function showStep(stepIndex) {
        // Hide all steps first
        formSteps.forEach((step, index) => {
            step.classList.add('hidden');
            step.classList.remove('animate-fade-in'); // Reset animation
        });

    // Show the current step with animation
    formSteps[stepIndex].classList.remove('hidden');
    formSteps[stepIndex].classList.add('animate-fade-in');

    // Update progress bar
    const progress = ((stepIndex + 1) / totalSteps) * 100;
    progressBar.style.width = `${progress}%`;
    stepIndicator.textContent = `Step ${stepIndex + 1} of ${totalSteps}`;

    // Update button visibility
    prevButton.classList.toggle('hidden', stepIndex === 0);
        nextButton.classList.toggle('hidden', stepIndex === totalSteps - 1);
        reviewbutton.classList.toggle('hidden', (stepIndex !== totalSteps - 1));
        submitButton.classList.toggle('hidden', (stepIndex !== totalSteps - 1));
        //console.log("userIdforbtn.value");
        //console.log(userIdforbtn.value);
        //if (userIdforbtn.value == 0) {
        //    submitButton.classList.toggle('hidden', true);
        //}

    // Adjust button margin for first step
    if (stepIndex === 0) {
        nextButton.classList.add('ml-auto');
                } else {
        nextButton.classList.remove('ml-auto');
                }

    // Apply slide effect
 //   formStepContainer.style.transform = `translateX(-${stepIndex * 100}%)`;

  
    
            }

    // Function to clear all error messages
    function clearErrors() {
        document.querySelectorAll('.text-red-500').forEach(errorElement => {
            errorElement.classList.add('hidden');
        });
                document.querySelectorAll('input, select').forEach(input => {
        input.classList.remove('border-red-500');
    input.classList.add('border-gray-300');
                });
            }

    // Function to display an error message
    function displayError(fieldId, message) {
                const errorElement = document.getElementById(`error-${fieldId}`);
    const inputElement = document.getElementById(fieldId);
    if (errorElement) {
        errorElement.textContent = message;
    errorElement.classList.remove('hidden');
                }
    if (inputElement) {
        inputElement.classList.add('border-red-500');
    inputElement.classList.remove('border-gray-300');
                }
            }

            // Validation logic for each step
            function validateCurrentStep() {
                return true;
            }
   // function validateCurrentStep() {
   //     clearErrors();
   // let isValid = true;

   // if (currentStep === 0) { // Step 1: Personal Information
   //                 const name = document.getElementById('name').value.trim();
   // const email = document.getElementById('email').value.trim();

   // if (!name) {
   //     displayError('name', 'Name is required');
   // isValid = false;
   //                 }
   // if (!email) {
   //     displayError('email', 'Email is required');
   // isValid = false;
   //                 } else if (!/\S+@\S+\.\S+/.test(email)) {
   //     displayError('email', 'Email is invalid');
   // isValid = false;
   //                 }
   // formData.name = name;
   // formData.email = email;
   //             } else if (currentStep === 1) { // Step 2: Address Information
   //                 const address1 = document.getElementById('address1').value.trim();
   // const city = document.getElementById('city').value.trim();
   // const zip = document.getElementById('zip').value.trim();

   // if (!address1) {
   //     displayError('address1', 'Address Line 1 is required');
   // isValid = false;
   //                 }
   // if (!city) {
   //     displayError('city', 'City is required');
   // isValid = false;
   //                 }
   // if (!zip) {
   //     displayError('zip', 'Zip Code is required');
   // isValid = false;
   //                 } else if (!/^\d{5}(-\d{4})?$/.test(zip)) {
   // //    displayError('zip', 'Zip Code is invalid');
   //// isValid = false;
   //                 }
   // formData.address1 = address1;
   // formData.city = city;
   // formData.zip = zip;
   //             } else if (currentStep === 2) { // Step 3: Preferences
   //     formData.favoriteColor = document.getElementById('favoriteColor').value;
   // formData.newsletter = document.getElementById('newsletter').checked;
   //             }
   // // No validation needed for the review step

   // return isValid;
   //         }

    // Populate the review step with collected data
 

            // Event listener for Next button
            nextButton.addEventListener('click', () => {
                if (validateCurrentStep()) {
        currentStep++;
    showStep(currentStep);
                }
            });

            // Event listener for Previous button
            prevButton.addEventListener('click', () => {
        currentStep--;
    showStep(currentStep);
            });


        reviewbutton.addEventListener('click', () => {
            document.getElementById('hidBtnclickfrom').value = 1;
                document.getElementById('frmForms').submit();
        });

            // Event listener for Submit button
        submitButton.addEventListener('click', () => {
            document.getElementById('hidBtnclickfrom').value = 0;
                console.log('user id value ', userIdforbtn.value);
                if (userIdforbtn.value == 0) {
                    $("#registerMessage").text('');
                    $('#myPopup').show();
                    $('#overlay').show();

                   
                }
                else {
                    console.log('Form Submitted!', formData);
                    document.getElementById('frmForms').submit();
                }

              
        // In a real application, you would send formData to a server
      
    // Using a custom message box instead of alert()
    //const messageBox = document.createElement('div');
    //messageBox.className = 'fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50';
    //messageBox.innerHTML = `
    //<div class="bg-white p-8 rounded-xl shadow-lg text-center max-w-sm w-full">
    //    <h3 class="text-2xl font-bold text-gray-800 mb-4">Form Submitted!</h3>
    //    <p class="text-gray-700 mb-6">Your data has been successfully processed.</p>
    //    <button id="close-message-box" class="px-6 py-3 bg-blue-600 text-white rounded-xl font-semibold hover:bg-blue-700 transition-colors duration-300 shadow-md">
    //        Close
    //    </button>
    //</div>
    //`;
    //document.body.appendChild(messageBox);

        //        document.getElementById('close-message-box').addEventListener('click', () => {
        //document.body.removeChild(messageBox);
        //        });
             });

    // Initial display of the first step
    showStep(currentStep);
        });
 