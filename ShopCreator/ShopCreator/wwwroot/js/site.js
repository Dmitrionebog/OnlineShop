
var selectedOptions = [];
var namesOfSelect = [];
var Properties = [];


function addNewFields(PropertiesFromView) {
   Properties = PropertiesFromView;
  addFields();
  updateSelectsAfterClick();
}

function updateSelectsAfterClick() {
  getAllSelectedOptionsAndNamesOfSelect();
  drawOptionsForEverySelect();
  buttonIsVisiable();

}

function getAllSelectedOptionsAndNamesOfSelect() {
  var AllSelectLists = document.getElementsByClassName('idInput');
  selectedOptions = [];
  namesOfSelect = [];
  for (let index = 0; index < AllSelectLists.length; index++) {
    if (AllSelectLists[index].options[AllSelectLists[index].selectedIndex] !== null) {
      var IdSelectProperty = AllSelectLists[index].options[AllSelectLists[index].selectedIndex].value;
      selectedOptions.push(IdSelectProperty);
      var NameOfSelect = AllSelectLists[index].name;
      namesOfSelect.push(NameOfSelect);
    }
  }
}

function drawOptionsForEverySelect() {
  for (var index = 0; index < namesOfSelect.length; index++) {
    var currentSelect = document.getElementsByName(namesOfSelect[index]);
    //удаление всех option у select кроме выбранного
    var length = currentSelect[0].options.length;
    var selectedIndex = currentSelect[0].options.selectedIndex;
    for (i = length - 1; i >= 0; i--) {
      if (i !== selectedIndex) {
        currentSelect[0].options[i] = null;
      }
    }

    //запись всех option у select кроме уже выбранных
    for (var i = 0; i < Properties.length; i++) {
      var entryIndex = selectedOptions.indexOf(Properties[i].Id);
      if (entryIndex === -1) {
        var option = document.createElement("option");
        option.value = Properties[i].Id;
        option.text = Properties[i].Name;
        currentSelect[0].appendChild(option);
      }
    }
  }
}

function addFields() {

  var countproperties = document.getElementsByClassName('idInput').length;

  var PropertiesContainer = document.getElementById("divPropertiesContainer");

  var PropertyContainer = document.createElement("div");
  PropertiesContainer.appendChild(PropertyContainer);

  var div1 = document.createElement("div");
  div1.className += ' myClass';
  PropertyContainer.appendChild(div1);

  var label = document.createElement("Label");
  label.innerHTML = "Name of Property";
  div1.appendChild(label);




  getProperties();
  // var Properties = [{ Id: 1, Name: "abcd", Type: 1 }, { Id: 2, Name: "dbca", Type: 2 }];




  var selectList = document.createElement("select");
  selectList.classList.add("idInput", "form-control");
  selectList.name = `PropertiesOfProduct[${countproperties}].PropertyId`;
  div1.appendChild(selectList);
  //selectList.onchange = function () { drawCurrentValueInput(this.selectList.name); }
  //var currentIndex = countproperties;
    selectList.onchange = function () { drawCurrentValueInput.apply(this);  }


  for (var i = 0; i < Properties.length; i++) {
    if (selectedOptions.indexOf(Properties[i].Id) === -1) {
      var option = document.createElement("option");
      option.value = Properties[i].Id;
      option.text = Properties[i].Name;
      selectList.appendChild(option);
    }
  }

  // for (var i = 0; i < Properties.length; i++) {
  //    var option = document.createElement("option");
  //    option.value = Properties[i].Id;
  //    option.text = Properties[i].Name;
  //    selectList.appendChild(option);
  //}




  var div2 = document.createElement("div");
  div2.className += ' myClass';
  PropertyContainer.appendChild(div2);

  var label2 = document.createElement("Label");
  label2.innerHTML = "Value of Property";
  div2.appendChild(label2);

    var elementAndSelectedProperty = drawCurrentValueInput(countproperties);

    div2.appendChild(elementAndSelectedProperty.element);
   

    
    addValidation(elementAndSelectedProperty);


  var button = document.createElement("button");
  button.className += ' myClass2';
  button.innerHTML = "X";

  button.onclick = Close
  // span.addEventListener("click", Close);

  PropertyContainer.appendChild(button);



  div2.appendChild(document.createElement("br"));
  countproperties++;
  return false;

}

function drawCurrentValueInput(countproperties) {

  if (typeof countproperties === "undefined") {
    var itIsChangeOperation = true;
    var countpropertiesStr = this.name;
     countproperties = parseInt(countpropertiesStr.match(/\d+/));
  }

  var currentSelect = document.getElementsByName(`PropertiesOfProduct[${countproperties}].PropertyId`);
  var idOfSelectedProperty = currentSelect[0].options[currentSelect[0].options.selectedIndex].value;
  var selectedProperty = Properties.find(prop => prop.Id === idOfSelectedProperty);
  var element;
  switch (selectedProperty.Type) {
    case "0":
      element = document.createElement("input");
      element.type = "text";
      element.name = `PropertiesOfProduct[${countproperties}].Value`;
          element.classList.add("valueInput", "form-control");
          
      break;
    case "1":
      element = document.createElement("input");
      element.type = "text";
      element.name = `PropertiesOfProduct[${countproperties}].Value`;
      element.classList.add("valueInput", "form-control");
      break;  
 
    case "2":
      element = document.createElement("input");
      element.type = "number";
      element.name = `PropertiesOfProduct[${countproperties}].Value`;
      element.classList.add("valueInput", "form-control");
      break;
    case "3":
      element = document.createElement("input");
      element.name = `PropertiesOfProduct[${countproperties}].Value`;
      element.classList.add("valueInput", "form-control");
      break;
    case "4":
      element = document.createElement("select");
      element.name = `PropertiesOfProduct[${countproperties}].Value`;
      element.classList.add("valueInput", "form-control");
      var arrayOfOptions = selectedProperty.ValidValues.split(';');
          for (var indexOfOption = 0; indexOfOption < arrayOfOptions.length; indexOfOption++) {
        var option = document.createElement("option");
        option.value = indexOfOption;
        option.text = arrayOfOptions[indexOfOption];
        element.appendChild(option);
          }
          
      break;
    case "5":
      element = document.createElement("input");
      element.type = "checkbox";
      element.name = `PropertiesOfProduct[${countproperties}].Value`;
      element.classList.add("valueInput", "checkboxInput");
      element.style.width = "100%";
 
      break;
  }

    var elementAndSelectedProperty = {
        element: element,
        selectedProperty: selectedProperty
    };

  if (itIsChangeOperation) {
      var oldElements = document.getElementsByName(`PropertiesOfProduct[${countproperties}].Value`);
    var oneOldElement = oldElements[0];
      oneOldElement.parentNode.replaceChild(element, oneOldElement);

      addValidation(elementAndSelectedProperty);

      
  }

   
    return elementAndSelectedProperty;
}

function addValidation(elementAndSelectedProperty)
{
    var rulesFromCase = [];
    var arrayOfStrings = elementAndSelectedProperty.selectedProperty.ValidValues.split(';');
    switch (elementAndSelectedProperty.selectedProperty.Type) {

        case "0": case "4": 
            rulesFromCase[0] = {
                attribute: "required",
                value: true
            };
            break;
        case "1":
            
            rulesFromCase[0] = {
                attribute: "minlength",
                value: arrayOfStrings[0]
            };
            rulesFromCase[1] = {
                attribute: "maxlength",
                value: arrayOfStrings[1]
            };
            rulesFromCase[2] = {
                attribute: "required",
                value: true
            };
            break;

        case "2":
            rulesFromCase[0] = {
                attribute: "required",
                value: true
            };
            rulesFromCase[1] = {
                attribute: "digits",
                value: true
            };
            rulesFromCase[2] = {
                attribute: "min",
                value: parseInt(arrayOfStrings[0],10)
            };
            rulesFromCase[3] = {
                attribute: "max",
                value: parseInt(arrayOfStrings[1], 10)
            };
           
            break;
        case "3":
            rulesFromCase[0] = {
                attribute: "min",
                value: (parseFloat(arrayOfStrings[0]) * 1.0)
            };
            rulesFromCase[1] = {
                attribute: "max",
                value: parseFloat(arrayOfStrings[1])
            };
            rulesFromCase[2] = {
                attribute: "required",
                value: true
            };
            
            break;

    }
    
    var validator = $('#myForm').validate();
        $(elementAndSelectedProperty.element).rules("remove");
        
        //validator.resetForm();
    rulesFromCase.forEach(function (ruleFromCase) {
        var rule = {};
        var key = ruleFromCase.attribute;
        var value = ruleFromCase.value;
        rule[key] = value;
        $(elementAndSelectedProperty.element).rules("add", rule);
    }); 
}

function addAllValidation(PropertiesFromView) {
    var idInputs = document.querySelectorAll(".idInput");
    Properties = PropertiesFromView;
    for (var index = 0; index < idInputs.length; index++) {
        var element = document.getElementsByName(`PropertiesOfProduct[${index}].Value`)[0];
        var currentSelect = document.getElementsByName(`PropertiesOfProduct[${index}].PropertyId`);
        var idOfSelectedProperty = currentSelect[0].options[currentSelect[0].options.selectedIndex].value;
        var selectedProperty = Properties.find(prop => prop.Id === idOfSelectedProperty);
        var elementAndSelectedProperty = {
            element: element,
            selectedProperty: selectedProperty
        };
        addValidation(elementAndSelectedProperty);

    }
}

    function Close() {

        //удаление текущего элемента из массивов namesOfSelect и selectedOptions
        var nameOfDeletedSelect = this.parentNode.getElementsByTagName('select')[0].name;
        var indexOfDeletedSelect = namesOfSelect.indexOf(nameOfDeletedSelect);
        //удаление правил валидация этого input
        var deletedInput = document.getElementsByName(`PropertiesOfProduct[${indexOfDeletedSelect}].Value`);
        $(deletedInput).rules("remove");

        namesOfSelect.splice(indexOfDeletedSelect, 1);
        selectedOptions.splice(indexOfDeletedSelect, 1);

        this.parentNode.parentNode.removeChild(this.parentNode);

        updateIds(indexOfDeletedSelect);
        //document.getElementById('divPropertiesContainer').style.display = 'none';
    }

    function updateIds(indexOfDeletedSelect) {
        //нашли все inputs и selects
        var idInputs = document.querySelectorAll(".idInput");
        var valueInputs = document.querySelectorAll(".valueInput");

        //удаляем rules последнего элемента
        var lastElement = document.getElementsByName(`PropertiesOfProduct[${idInputs.length}].Value`);
        $(lastElement[0]).rules("remove");
        //переназначаем validation rules для всех input
        for (var index = indexOfDeletedSelect; index < idInputs.length; index++) {
            var nextElements = document.getElementsByName(`PropertiesOfProduct[${index + 1}].Value`);
            var nextElement = nextElements[0];
            nextElement.setAttribute('name', `PropertiesOfProduct[${index}].Value`);
            var currentSelect = document.getElementsByName(`PropertiesOfProduct[${index + 1}].PropertyId`);
            var idOfSelectedProperty = currentSelect[0].options[currentSelect[0].options.selectedIndex].value;
            var selectedProperty = Properties.find(prop => prop.Id === idOfSelectedProperty);
            currentSelect[0].setAttribute('name', `PropertiesOfProduct[${index}].PropertyId`);

            //И наконец переназначение id и  for у label для ошибок,которые идут после удаляемого
            var nextLabel = document.getElementById(`PropertiesOfProduct[${index + 1}].Value-error`);
            if (nextLabel) {
                nextLabel.setAttribute("for", `PropertiesOfProduct[${index}].Value`);
                nextLabel.setAttribute("id", `PropertiesOfProduct[${index}].Value-error`);
            }
            var elementAndSelectedProperty = {
                element: nextElement,
                selectedProperty: selectedProperty
            };
            addValidation(elementAndSelectedProperty);
        }
    }

    function buttonIsVisiable() {
        var button = document.getElementById("btnAddPropery");
        var countproperties = document.querySelectorAll(".idInput");
        //var isButtonVisiable = true;
        if (countproperties.length === Properties.length && countproperties.length > 0) {
            button.style = "display: none";
            // isButtonVisiable = false;
        }
        else {
            button.style = "display: block";
        }

    }

    function beforeSubmit()
    {
        removeUnnecessaryCheckboxs();
        removeUnnecessaryNames();
        myValidationCategories();
    }

function removeUnnecessaryCheckboxs()
{
    var checkboxInputs = document.getElementsByClassName('checkboxInput');
    //var errorLabels = document.getElementsByClassName('error');
    var isValid = $('#myForm').valid();
    if (isValid) {
        for (var i = 0; i < checkboxInputs.length; i++) {

            if (!checkboxInputs[i].checked) {
                checkboxInputs[i].checked = true;
                checkboxInputs[i].value = "off";
            }
        }
    }
}

function removeUnnecessaryNames() {
    var divs = document.getElementsByClassName("oneCategory");
    for (var div of divs) {
        var selects = div.children;
        for (i = 0; i < selects.length - 2; i++) {
            selects[i].removeAttribute("name");
        }
    }
}


async function AddCategories(isItEditOperation, id, dropdown) {
    if (isItEditOperation)
    {
        if (id === undefined) { var url = "../../Categories/GetCategories"; }
        else { var url = "../../Categories/GetCategories/" + id; }
    }
    else
    {
        if (id === undefined) { var url = "../Categories/GetCategories"; }
        else { var url = "../Categories/GetCategories/" + id; }
    }

    var data = await GetDataFromServer(url);
    DrawDropdownWithData(isItEditOperation, data, dropdown);
    button2IsVisiable(isItEditOperation);
}

function DrawDropdownWithData(isItEditOperation,categories, currentSelect) {
    var dropdown = document.createElement('select');
    dropdown.classList.add("form-control");
    dropdown.name = `IdsOfCategories`;
    dropdown.addEventListener("change", reDrawSelectsAfterClick);
    dropdown.isItEditOperation = isItEditOperation;
    var CategoriesContainer = document.getElementById("divCategoriesContainer");
    if (currentSelect == undefined) {
        var OneCategory = document.createElement("div");
        OneCategory.style.display = "flex";
        OneCategory.className = 'oneCategory';
        OneCategory.appendChild(dropdown);

        var xButton = document.createElement("button");
        //button.className += ' myClass2';
        xButton.innerHTML = "X";
        xButton.type = "button";
        xButton.onclick = DeleteChain.bind(this);
        //xButton.addEventListener("click", DeleteChain(this));
        //xButton.onclick = DeleteChain();
        OneCategory.appendChild(xButton);

        CategoriesContainer.appendChild(OneCategory);
    }
    else {
        var OneCategory = currentSelect.closest("div");
        var xButton = OneCategory.lastElementChild;
        OneCategory.style.display = "flex";
        OneCategory.className = 'oneCategory';
        //OneCategory.appendChild(dropdown);
        OneCategory.insertBefore(dropdown, xButton);

    }
    dropdown.onchange = function () { IsMoreDropdown(isItEditOperation,dropdown.options[dropdown.selectedIndex], dropdown) }
    //dropdown.onchange = function (ev) { IsMoreDropdown.apply(this); }

    //Добавляем Default Option
    var option = document.createElement("option");
    option.value = 0;
    option.text = "Please , choose category";
    dropdown.appendChild(option);

    for (var i = 0; i < categories.length; i++) {
        var option = document.createElement("option");
        option.value = categories[i].id;
        option.text = categories[i].name;
        if (categories[i].hasChilds == true) { option.setAttribute("data-haschilds", true) }
        dropdown.appendChild(option);
    }
}

function IsMoreDropdown(isItEditOperation,CheckedOption, dropdown) {
    if (CheckedOption.hasAttribute('data-haschilds')) {
        AddCategories(isItEditOperation,CheckedOption.value, dropdown);
    }
}

async function GetDataFromServer(url) {
    // отправляет запрос и получаем ответ

        var response = await fetch(url, {
            method: "GET",
            headers: { "Accept": "application/json" }
        });




    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const data = await response.json();
        return data;
    }
}

function DeleteChain(context) {
    context.path[1].remove();
}

function myValidationCategories(e)
{
    //удаляем старые сообщения об ошибке
    //var labels = document.getElementsByClassName('my-validation').length!= 0 ? (document.getElementsByClassName('my-validation')): [];
    var labels = document.getElementsByClassName('my-validation');
    if (labels.length != 0)
    {
        for (var label of labels) { label.remove(); }
        label.remove();
    }

    //проверяем чтобы не было одинаковых цепочек
    var lastSelects = document.getElementsByName('IdsOfCategories');
    checkedOptions = [];
    var repeatCategory = false;
    for (var sel of lastSelects)
    {
        var checkedOption = sel.options[sel.selectedIndex].value;
        if (checkedOptions.indexOf(checkedOption) != -1) { repeatCategory = true; }
        checkedOptions.push(checkedOption);
    }

    if (repeatCategory)
    {
        var submitButton = document.getElementById('btnAddCategory');
        var label  = document.createElement('label');
            label.className = "my-validation";
            label.innerHTML = "Please , remove duplicate chains .";
        submitButton.parentNode.before(label);
    }

    //проверяем заполненность всех select
    console.log(this.firstElementChild);
    console.log(this.target);
    var currentTargetSelect = this.firstElementChild.firstElementChild;
    var selects = document.querySelectorAll("div.oneCategory > select");
    for (var select of selects)
    {
        if ((select.value == 0) && (select != currentTargetSelect))
        {
            select.onclick = myValidationCategories;
            //select.addEventListener("focusout", myValidationCategories);
            let label  = document.createElement('label');
            label.className = "my-validation";
            label.innerHTML = "Please,choose value of select.";
            select.parentNode.after(label);
            return false;
        }
    }
    return true;
}

function reDrawSelectsAfterClick()
{   

    var siblings = [];
    var sibling = this;
    while (sibling.nextElementSibling.nextElementSibling) {
        sibling = sibling.nextSibling;
        siblings.push(sibling);
    }
    for (var sibling of siblings)
    {
        sibling.remove();
    }
    //IsMoreDropdown(this.isItEditOperation, this.options[this.selectedIndex], this);
}

function button2IsVisiable(isItEditOperation)
{
    var button = document.getElementById("btnAddCategory");
    if (button != null)
    {
        var divs = document.getElementsByClassName("oneCategory");
        var countCategories = (isItEditOperation) ? divs[0].firstElementChild.options.length : (divs[0].firstElementChild.options.length - 1);
        if (divs.length > 0 && divs.length === countCategories) {
            button.style = "display: none";
            // isButtonVisiable = false;
        }
        else {
            button.style = "display: block";
        }
    }
}




   