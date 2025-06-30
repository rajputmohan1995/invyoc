const EXPIRY_HOURS = 24;
var thisWeekRestDay = 0;

const weekdays = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

const mealPlan = {
    Morning: [
        { item: "3 Boiled Eggs", img: "images/3-boiled-eggs.jpg" },
        { item: "1 Whole Grain Roti", img: "images/1-whole-grain-roti.jpg" },
        { item: "1 Banana", img: "images/1banana.jpg" },
        { item: "Black Coffee/Green Tea", img: "images/green-tea-coffee.webp" }
    ],
    "Mid-Morning": [
        { item: "100g Yogurt", img: "images/yogurt.jpg" },
        { item: "10 almonds", img: "images/10-almonds.jpg" },
        { item: "1 Whey Protein Scoop", img: "images/whey-protein-scoop.jpg" },
    ],
    Afternoon: [
        { item: "150g Grilled Chicken/Tofu/Paneer", img: "images/grill-tofu.jpg" },
        { item: "2 Rotis + Sabji/Dal", img: "images/2-Roti-Sabji.jpg" },
        { item: "Mixed veggies", img: "images/Mixed-Veggies.jpg" },
        { item: "Salad", img: "images/salad.jpg" },
    ],
    Evening: [
        { item: "1 boiled egg or protein shake", img: "images/1-egg-or-protien-shake.jpg" },
        { item: "1 fruit (apple/orange)", img: "images/fruit.jpg" },
    ],
    Night: [
        { item: "150g Chicken/Tofu/Paneer", img: "images/grill-tofu.jpg" },
        { item: "1 Roti or 1/2 Cup Rice", img: "images/1roti-half-rice.jpeg" },
        { item: "Steamed veggies or soup", img: "images/vegetable-soup.jpg" },
    ],
};

const workOutVideos = ["wIynl3at0Rs", "TIFHRwGFNXg", "UIPvIYsjfpo", "-hSma-BRzoo", "bO_NwLKBxf4", "j57HMjVM7Is", "futxulTiq54"];

const timeSlots = [
    { label: "Morning", from: 5, to: 9 },
    { label: "Mid-Morning", from: 10, to: 11 },
    { label: "Afternoon", from: 12, to: 16 },
    { label: "Evening", from: 17, to: 20 },
    { label: "Night", from: 21, to: 23 },
];

function switchTab(index) {
    const tabs = document.querySelectorAll(".tab");
    const contents = document.querySelectorAll(".tab-content");
    tabs.forEach((t, i) => t.classList.toggle("active", i === index));
    contents.forEach((c, i) => c.classList.toggle("active", i === index));
}

function updatePlan() {
    const day = document.getElementById("day").value;
    const time = document.getElementById("time").value;
    const mealItems = mealPlan[time]
        .map((m) => `<div class='meal-item'><div>${m.item}</div><img src='${m.img}' alt='${m.item}' /></div>`)
        .join("");
    document.getElementById("meal").innerHTML = mealItems;

    let videoTagContent = "<div><h3>No workout required today. Take rest is possible.</h3></div>";

    if (day != weekdays[0] && day != thisWeekRestDay) {
        const workoutVideoId = workOutVideos[getKeyValue()];
        videoTagContent = `<div"><iframe src='https://www.youtube.com/embed/${workoutVideoId}' allowfullscreen></iframe></div>`;
    }

    document.getElementById("workoutVideos").innerHTML = videoTagContent;
}

function toggleVisibility(id) {
    const section = document.getElementById(id);
    section.style.display = section.style.display === "block" ? "none" : "block";
}

function getTodayKey() {
    const today = new Date();
    return today.toISOString().split("T")[0]; // YYYY-MM-DD format
}

function getKeyValue() {
    const key = getTodayKey();
    var data = localStorage.getItem(key);
    if (data) {
        var item = JSON.parse(data);
        if (item && item.val) return item.val;
    } else return getRandomIndex(workOutVideos.length);
}

function getOrSetTodayWorkout() {
    const key = getTodayKey();
    let savedValue = localStorage.getItem(key);

    if (savedValue) {
        const data = JSON.parse(savedValue);
        const now = new Date().getTime();
        const ageInHours = (now - data.timestamp) / (1000 * 60 * 60);

        if (ageInHours > EXPIRY_HOURS)
            localStorage.removeItem(key);
    }

    savedValue = localStorage.getItem(key);
    if (!savedValue) {
        localStorage.setItem(
            key,
            JSON.stringify({ val: getRandomIndex(workOutVideos.length), timestamp: new Date().getTime() })
        );
    }
}

function getRandomIndex(maxValue) {
    return Math.floor(Math.random() * maxValue);
}

function clearTodaySavedWorkoutVideo() {
    localStorage.clear();
    location.reload();
}

function setupTodayDayAndTime() {
    const date = new Date();
    const hour = date.getHours();
    const day = date.getDay();

    let innerHtml = "";
    weekdays.forEach((day) => { innerHtml += `<option>${day}</option>`; });
    document.getElementById("day").innerHTML = innerHtml;

    innerHtml = "";

    timeSlots.forEach((timeOfDay) => { innerHtml += `<option>${timeOfDay.label}</option>`; });

    document.getElementById("time").innerHTML = innerHtml;
    document.getElementById("day").value = weekdays[day];

    const slot = timeSlots.find((t) => hour >= t.from && hour <= t.to);
    if (slot) document.getElementById("time").value = slot.label;
}

function setOrGetThisWeekRestDay() {
    thisWeekRestDay = localStorage.getItem("ThisWeekRestDay");
    let day = new Date().getDay();

    if (day === 0 || !thisWeekRestDay) {
        var randomWorkoutIndex = getRandomIndex(weekdays.length - 1);
        if (randomWorkoutIndex <= 0) setOrGetThisWeekRestDay();
        else {
            thisWeekRestDay = weekdays[randomWorkoutIndex];
            localStorage.setItem("ThisWeekRestDay", thisWeekRestDay);
        }
    }
}

// Set current weekday and time
window.onload = () => {
    setOrGetThisWeekRestDay();
    getOrSetTodayWorkout();
    setupTodayDayAndTime();
    updatePlan();
};
