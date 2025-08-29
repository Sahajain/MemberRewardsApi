document.addEventListener('DOMContentLoaded', () => {
    const API_BASE_URL = 'https://localhost:7288/api'; // Change port 

    // State
    let jwtToken = null;
    let memberId = null;

    // UI Elements
    const mobileNumberInput = document.getElementById('mobileNumber');
    const otpInput = document.getElementById('otp');
    const registerBtn = document.getElementById('registerBtn');
    const verifyBtn = document.getElementById('verifyBtn');
    const authedSection = document.getElementById('authedSection');
    const memberIdDisplay = document.getElementById('memberIdDisplay');
    const pointsDisplay = document.getElementById('pointsDisplay');
    const refreshPointsBtn = document.getElementById('refreshPointsBtn');
    const purchaseAmountInput = document.getElementById('purchaseAmount');
    const addPointsBtn = document.getElementById('addPointsBtn');
    const redeemBtns = document.querySelectorAll('.redeemBtn');
    const messageArea = document.getElementById('messageArea');

    // Event Listeners
    registerBtn.addEventListener('click', handleRegister);
    verifyBtn.addEventListener('click', handleVerify);
    refreshPointsBtn.addEventListener('click', () => fetchMemberPoints());
    addPointsBtn.addEventListener('click', handleAddPoints);
    redeemBtns.forEach(btn => btn.addEventListener('click', handleRedeem));

    // --- Handlers ---
    async function handleRegister() {
        const mobileNumber = mobileNumberInput.value;
        if (!mobileNumber) {
            showMessage('Please enter a mobile number.', 'red');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/Members/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ mobileNumber })
            });

            const data = await response.json();
            if (response.ok) {
                showMessage(`Registration successful. OTP: ${data.otp} (check console/network tab in a real app)`, 'green');
            } else {
                throw new Error(data.message || 'Registration failed.');
            }
        } catch (error) {
            showMessage(error.message, 'red');
        }
    }

    async function handleVerify() {
        const mobileNumber = mobileNumberInput.value;
        const otp = otpInput.value;

        if (!mobileNumber || !otp) {
            showMessage('Please enter both mobile number and OTP.', 'red');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/Members/verify`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ mobileNumber, otp })
            });

            const data = await response.json();
            if (response.ok) {
                jwtToken = data.token;
                memberId = data.memberId;
                showMessage('Verification successful!', 'green');
                showAuthedSection();
                fetchMemberPoints();
            } else {
                throw new Error(data.message || 'Verification failed.');
            }
        } catch (error) {
            showMessage(error.message, 'red');
        }
    }

    async function fetchMemberPoints() {
        if (!jwtToken || !memberId) return;

        try {
            const response = await fetch(`${API_BASE_URL}/Points/${memberId}`, {
                headers: { 'Authorization': `Bearer ${jwtToken}` }
            });
            const data = await response.json();
            if (response.ok) {
                pointsDisplay.textContent = data.totalPoints;
            } else {
                throw new Error(data.message || 'Failed to fetch points.');
            }
        } catch (error) {
            showMessage(error.message, 'red');
        }
    }

    async function handleAddPoints() {
        const purchaseAmount = purchaseAmountInput.value;
        if (!purchaseAmount || purchaseAmount <= 0) {
            showMessage('Please enter a valid purchase amount.', 'red');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/Points/add`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${jwtToken}`
                },
                body: JSON.stringify({ memberId: parseInt(memberId), purchaseAmount: parseFloat(purchaseAmount) })
            });

            const data = await response.json();
            if (response.ok) {
                showMessage(data.message, 'green');
                fetchMemberPoints();
                purchaseAmountInput.value = '';
            } else {
                throw new Error(data.message || 'Failed to add points.');
            }
        } catch (error) {
            showMessage(error.message, 'red');
        }
    }

    async function handleRedeem(event) {
        const pointsToRedeem = event.target.dataset.points;

        try {
            const response = await fetch(`${API_BASE_URL}/Coupons/redeem`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${jwtToken}`
                },
                body: JSON.stringify({ memberId: parseInt(memberId), pointsToRedeem: parseInt(pointsToRedeem) })
            });

            const data = await response.json();
            if (response.ok) {
                showMessage(`Success! Redeemed coupon: ${data.coupon.couponCode} (Value: ₹${data.coupon.value})`, 'green');
                fetchMemberPoints();
            } else {
                throw new Error(data.message || 'Redemption failed.');
            }
        } catch (error) {
            showMessage(error.message, 'red');
        }
    }


    // --- UI Helpers ---
    function showMessage(message, color) {
        messageArea.textContent = message;
        messageArea.className = `mt-8 p-4 rounded-md text-center bg-${color}-100 text-${color}-800`;
    }

    function showAuthedSection() {
        authedSection.classList.remove('hidden');
        memberIdDisplay.textContent = memberId;
    }
});
