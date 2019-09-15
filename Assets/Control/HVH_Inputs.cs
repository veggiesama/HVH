// GENERATED AUTOMATICALLY FROM 'Assets/Control/HVH_Inputs.inputactions'

using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class HVH_Inputs : IInputActionCollection
{
    private InputActionAsset asset;
    public HVH_Inputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""HVH_Inputs"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""efc1e825-5654-4a2a-b1ad-9e56dc135c9a"",
            ""actions"": [
                {
                    ""name"": ""Navigate"",
                    ""type"": ""Value"",
                    ""id"": ""dea79564-ab25-4eed-82d0-01b296a9b1d2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""0ee65323-8feb-47c7-b782-6c2eed3f2b64"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""4cedae9a-7e4f-4fe2-bf73-e41b75131d01"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Point"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c5815761-31ae-4d3b-8ef5-4aa527f5ad84"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f8eb1be6-1e1b-4fb6-a071-b7357e93924d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ScrollWheel"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fc2c898a-4e4d-422a-9f50-1d5eb8682122"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MiddleClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""8e81e6f7-ddbe-40de-bc92-bc6a6e861b90"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fa4ff39c-3cd9-4e4a-8495-f2fa16b62b2a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDevicePosition"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e65f19c7-7522-4f9b-bdb2-594e05975b95"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDeviceOrientation"",
                    ""type"": ""PassThrough"",
                    ""id"": ""fa7ca85d-9ea5-4d6e-9f89-f5d1c0515868"",
                    ""expectedControlType"": ""Quaternion"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""TrackedDeviceSelect"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2a469676-3875-41c8-98f0-ca827d431abe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Stick"",
                    ""id"": ""809f371f-c5e2-4e7a-83a1-d867598f40dd"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""14a5d6e8-4aaf-4119-a9ef-34b8c2c548bf"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2db08d65-c5fb-421b-983f-c71163608d67"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8ba04515-75aa-45de-966d-393d9bbd1c14"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""fcd248ae-a788-4676-a12e-f4d81205600b"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fb8277d4-c5cd-4663-9dc7-ee3f0b506d90"",
                    ""path"": ""<Gamepad>/dpad"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Stick"",
                    ""id"": ""e25d9774-381c-4a61-b47c-7b6b299ad9f9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Navigate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3db53b26-6601-41be-9887-63ac74e79d19"",
                    ""path"": ""<Joystick>/stick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0cb3e13e-3d90-4178-8ae6-d9c5501d653f"",
                    ""path"": ""<Joystick>/stick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0392d399-f6dd-4c82-8062-c1e9c0d34835"",
                    ""path"": ""<Joystick>/stick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""942a66d9-d42f-43d6-8d70-ecb4ba5363bc"",
                    ""path"": ""<Joystick>/stick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Joystick"",
                    ""action"": ""Navigate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""9e92bb26-7e3b-4ec4-b06b-3c8f8e498ddc"",
                    ""path"": ""*/{Submit}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82627dcc-3b13-4ba9-841d-e4b746d6553e"",
                    ""path"": ""*/{Cancel}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c52c8e0b-8179-41d3-b8a1-d149033bbe86"",
                    ""path"": ""<Pointer>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4faf7dc9-b979-4210-aa8c-e808e1ef89f5"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""38c99815-14ea-4617-8627-164d27641299"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""ScrollWheel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""24066f69-da47-44f3-a07e-0015fb02eb2e"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""MiddleClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4c191405-5738-4d4b-a523-c6a301dbf754"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7236c0d9-6ca3-47cf-a6ee-a97f5b59ea77"",
                    ""path"": ""<XRController>/devicePosition"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TrackedDevicePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""23e01e3a-f935-4948-8d8b-9bcac77714fb"",
                    ""path"": ""<XRController>/deviceRotation"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TrackedDeviceOrientation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""932fe797-a0a9-4eef-bd2d-556b362e08d0"",
                    ""path"": ""<XRController>/trigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""TrackedDeviceSelect"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Player"",
            ""id"": ""3cba4785-1a9a-4bd7-b049-c7208e1789b8"",
            ""actions"": [
                {
                    ""name"": ""L-Click"",
                    ""type"": ""Button"",
                    ""id"": ""d8abd8f9-2982-48d8-9b80-91af431a4b34"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""R-Click"",
                    ""type"": ""Button"",
                    ""id"": ""607a3612-0af7-4404-ad32-959bc1ea9736"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""63176a41-c8b1-406b-a05d-9563dee1bbc3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Queue (Hold)"",
                    ""type"": ""Button"",
                    ""id"": ""3da575d6-3634-4ec8-9979-b443c2946abc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Zoom (Hold)"",
                    ""type"": ""Button"",
                    ""id"": ""68614634-49b9-44fd-9d6f-af7caa9b6c1c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Stop"",
                    ""type"": ""Button"",
                    ""id"": ""f1c7ffbf-c759-4e16-a784-238d9f0328e9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability 1"",
                    ""type"": ""Button"",
                    ""id"": ""b1d61971-dd28-4df5-9116-ed79ebece4e3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability 2"",
                    ""type"": ""Button"",
                    ""id"": ""925647dd-b29a-400f-86ad-b16149effe61"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability 3"",
                    ""type"": ""Button"",
                    ""id"": ""7cb15976-724a-4ebf-84cf-109200cda811"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability 4"",
                    ""type"": ""Button"",
                    ""id"": ""a6362ff6-2c06-4534-8f24-e5fbc9cab7ab"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability 5"",
                    ""type"": ""Button"",
                    ""id"": ""8f171a26-becf-4185-be3b-ae13859fd120"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ability 6"",
                    ""type"": ""Button"",
                    ""id"": ""10639dc5-5005-43bd-8b3f-88132883b2ca"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item 1"",
                    ""type"": ""Button"",
                    ""id"": ""0d01c9cf-45f7-47f3-a988-71224331d44b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item 2"",
                    ""type"": ""Button"",
                    ""id"": ""d7b271ee-8cf6-42eb-9fa5-3af958dbe1db"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item 3"",
                    ""type"": ""Button"",
                    ""id"": ""5be6c9c3-2782-423f-8e94-96d0cde5cd65"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item 4"",
                    ""type"": ""Button"",
                    ""id"": ""00487898-7dcc-4a02-94ab-7deef70317d9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item 5"",
                    ""type"": ""Button"",
                    ""id"": ""8c2a6ec2-e814-4b00-a485-03bcd7ef1180"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Item 6"",
                    ""type"": ""Button"",
                    ""id"": ""b33ff94a-714a-43ed-a227-2eae7cde7a36"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Test"",
                    ""type"": ""Button"",
                    ""id"": ""fc5d690c-cf06-4928-a330-7aca8410787a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""05f6913d-c316-48b2-a6bb-e225f14c7960"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard&Mouse"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec758f90-6735-4d33-bb4c-ed187438ac94"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""L-Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6adcf509-e395-4269-9291-d22f39a5b77c"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""R-Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4f9fd4de-40df-4581-a69b-90d21fe71bc3"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Queue (Hold)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f4e9b25a-0e06-47d6-be31-ed68433dd137"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Zoom (Hold)"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8ff3afc-33b6-47ac-ba40-1f65e3677263"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Stop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4b84e07d-3e95-4263-a7f9-3bcc855814b7"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""05dd1e80-7a48-4953-a3f1-43a11b8d0e7d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ddf361c9-da90-402b-b1d7-30c16031f8a5"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""75bdfcbe-2183-41fc-a8bd-9554e1cc08ab"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e1036411-402e-4ff0-9d1a-053e36708ff7"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability 5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0022f1a9-e82f-4cbb-b2cf-67f58578dea0"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Ability 6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9560040d-a3c5-4f5e-a0c9-b02a08309bdc"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Item 1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c0dc12c5-be2e-4810-8393-e888a4ef359d"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Item 2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""130470f5-6453-4b85-80ca-c9e9dd88d052"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Item 3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ef5166c6-b71e-4c52-b925-a0ef392d091d"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Item 4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e27e4290-409a-4ae1-8d03-23c786ff66fc"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Item 5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""61cab68e-1efb-4c22-859b-93b710da7bb9"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard&Mouse"",
                    ""action"": ""Item 6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""653928d5-a773-40e2-ae07-efc295dc8af7"",
                    ""path"": ""*/{Point}"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Test"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e3d79fb4-d810-4fd5-81f0-9a6666ffa56c"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Test"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard&Mouse"",
            ""bindingGroup"": ""Keyboard&Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Joystick"",
            ""bindingGroup"": ""Joystick"",
            ""devices"": [
                {
                    ""devicePath"": ""<Joystick>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // UI
        m_UI = asset.GetActionMap("UI");
        m_UI_Navigate = m_UI.GetAction("Navigate");
        m_UI_Submit = m_UI.GetAction("Submit");
        m_UI_Cancel = m_UI.GetAction("Cancel");
        m_UI_Point = m_UI.GetAction("Point");
        m_UI_Click = m_UI.GetAction("Click");
        m_UI_ScrollWheel = m_UI.GetAction("ScrollWheel");
        m_UI_MiddleClick = m_UI.GetAction("MiddleClick");
        m_UI_RightClick = m_UI.GetAction("RightClick");
        m_UI_TrackedDevicePosition = m_UI.GetAction("TrackedDevicePosition");
        m_UI_TrackedDeviceOrientation = m_UI.GetAction("TrackedDeviceOrientation");
        m_UI_TrackedDeviceSelect = m_UI.GetAction("TrackedDeviceSelect");
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_LClick = m_Player.GetAction("L-Click");
        m_Player_RClick = m_Player.GetAction("R-Click");
        m_Player_Attack = m_Player.GetAction("Attack");
        m_Player_QueueHold = m_Player.GetAction("Queue (Hold)");
        m_Player_ZoomHold = m_Player.GetAction("Zoom (Hold)");
        m_Player_Stop = m_Player.GetAction("Stop");
        m_Player_Ability1 = m_Player.GetAction("Ability 1");
        m_Player_Ability2 = m_Player.GetAction("Ability 2");
        m_Player_Ability3 = m_Player.GetAction("Ability 3");
        m_Player_Ability4 = m_Player.GetAction("Ability 4");
        m_Player_Ability5 = m_Player.GetAction("Ability 5");
        m_Player_Ability6 = m_Player.GetAction("Ability 6");
        m_Player_Item1 = m_Player.GetAction("Item 1");
        m_Player_Item2 = m_Player.GetAction("Item 2");
        m_Player_Item3 = m_Player.GetAction("Item 3");
        m_Player_Item4 = m_Player.GetAction("Item 4");
        m_Player_Item5 = m_Player.GetAction("Item 5");
        m_Player_Item6 = m_Player.GetAction("Item 6");
        m_Player_Test = m_Player.GetAction("Test");
    }

    ~HVH_Inputs()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Navigate;
    private readonly InputAction m_UI_Submit;
    private readonly InputAction m_UI_Cancel;
    private readonly InputAction m_UI_Point;
    private readonly InputAction m_UI_Click;
    private readonly InputAction m_UI_ScrollWheel;
    private readonly InputAction m_UI_MiddleClick;
    private readonly InputAction m_UI_RightClick;
    private readonly InputAction m_UI_TrackedDevicePosition;
    private readonly InputAction m_UI_TrackedDeviceOrientation;
    private readonly InputAction m_UI_TrackedDeviceSelect;
    public struct UIActions
    {
        private HVH_Inputs m_Wrapper;
        public UIActions(HVH_Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Navigate => m_Wrapper.m_UI_Navigate;
        public InputAction @Submit => m_Wrapper.m_UI_Submit;
        public InputAction @Cancel => m_Wrapper.m_UI_Cancel;
        public InputAction @Point => m_Wrapper.m_UI_Point;
        public InputAction @Click => m_Wrapper.m_UI_Click;
        public InputAction @ScrollWheel => m_Wrapper.m_UI_ScrollWheel;
        public InputAction @MiddleClick => m_Wrapper.m_UI_MiddleClick;
        public InputAction @RightClick => m_Wrapper.m_UI_RightClick;
        public InputAction @TrackedDevicePosition => m_Wrapper.m_UI_TrackedDevicePosition;
        public InputAction @TrackedDeviceOrientation => m_Wrapper.m_UI_TrackedDeviceOrientation;
        public InputAction @TrackedDeviceSelect => m_Wrapper.m_UI_TrackedDeviceSelect;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                Navigate.started -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                Navigate.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                Navigate.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnNavigate;
                Submit.started -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                Submit.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                Submit.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnSubmit;
                Cancel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                Cancel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                Cancel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnCancel;
                Point.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                Point.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                Point.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPoint;
                Click.started -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                Click.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                Click.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnClick;
                ScrollWheel.started -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                ScrollWheel.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                ScrollWheel.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnScrollWheel;
                MiddleClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                MiddleClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                MiddleClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMiddleClick;
                RightClick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                RightClick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                RightClick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnRightClick;
                TrackedDevicePosition.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                TrackedDevicePosition.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                TrackedDevicePosition.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDevicePosition;
                TrackedDeviceOrientation.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                TrackedDeviceOrientation.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                TrackedDeviceOrientation.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceOrientation;
                TrackedDeviceSelect.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceSelect;
                TrackedDeviceSelect.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceSelect;
                TrackedDeviceSelect.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTrackedDeviceSelect;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                Navigate.started += instance.OnNavigate;
                Navigate.performed += instance.OnNavigate;
                Navigate.canceled += instance.OnNavigate;
                Submit.started += instance.OnSubmit;
                Submit.performed += instance.OnSubmit;
                Submit.canceled += instance.OnSubmit;
                Cancel.started += instance.OnCancel;
                Cancel.performed += instance.OnCancel;
                Cancel.canceled += instance.OnCancel;
                Point.started += instance.OnPoint;
                Point.performed += instance.OnPoint;
                Point.canceled += instance.OnPoint;
                Click.started += instance.OnClick;
                Click.performed += instance.OnClick;
                Click.canceled += instance.OnClick;
                ScrollWheel.started += instance.OnScrollWheel;
                ScrollWheel.performed += instance.OnScrollWheel;
                ScrollWheel.canceled += instance.OnScrollWheel;
                MiddleClick.started += instance.OnMiddleClick;
                MiddleClick.performed += instance.OnMiddleClick;
                MiddleClick.canceled += instance.OnMiddleClick;
                RightClick.started += instance.OnRightClick;
                RightClick.performed += instance.OnRightClick;
                RightClick.canceled += instance.OnRightClick;
                TrackedDevicePosition.started += instance.OnTrackedDevicePosition;
                TrackedDevicePosition.performed += instance.OnTrackedDevicePosition;
                TrackedDevicePosition.canceled += instance.OnTrackedDevicePosition;
                TrackedDeviceOrientation.started += instance.OnTrackedDeviceOrientation;
                TrackedDeviceOrientation.performed += instance.OnTrackedDeviceOrientation;
                TrackedDeviceOrientation.canceled += instance.OnTrackedDeviceOrientation;
                TrackedDeviceSelect.started += instance.OnTrackedDeviceSelect;
                TrackedDeviceSelect.performed += instance.OnTrackedDeviceSelect;
                TrackedDeviceSelect.canceled += instance.OnTrackedDeviceSelect;
            }
        }
    }
    public UIActions @UI => new UIActions(this);

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_LClick;
    private readonly InputAction m_Player_RClick;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_QueueHold;
    private readonly InputAction m_Player_ZoomHold;
    private readonly InputAction m_Player_Stop;
    private readonly InputAction m_Player_Ability1;
    private readonly InputAction m_Player_Ability2;
    private readonly InputAction m_Player_Ability3;
    private readonly InputAction m_Player_Ability4;
    private readonly InputAction m_Player_Ability5;
    private readonly InputAction m_Player_Ability6;
    private readonly InputAction m_Player_Item1;
    private readonly InputAction m_Player_Item2;
    private readonly InputAction m_Player_Item3;
    private readonly InputAction m_Player_Item4;
    private readonly InputAction m_Player_Item5;
    private readonly InputAction m_Player_Item6;
    private readonly InputAction m_Player_Test;
    public struct PlayerActions
    {
        private HVH_Inputs m_Wrapper;
        public PlayerActions(HVH_Inputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @LClick => m_Wrapper.m_Player_LClick;
        public InputAction @RClick => m_Wrapper.m_Player_RClick;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @QueueHold => m_Wrapper.m_Player_QueueHold;
        public InputAction @ZoomHold => m_Wrapper.m_Player_ZoomHold;
        public InputAction @Stop => m_Wrapper.m_Player_Stop;
        public InputAction @Ability1 => m_Wrapper.m_Player_Ability1;
        public InputAction @Ability2 => m_Wrapper.m_Player_Ability2;
        public InputAction @Ability3 => m_Wrapper.m_Player_Ability3;
        public InputAction @Ability4 => m_Wrapper.m_Player_Ability4;
        public InputAction @Ability5 => m_Wrapper.m_Player_Ability5;
        public InputAction @Ability6 => m_Wrapper.m_Player_Ability6;
        public InputAction @Item1 => m_Wrapper.m_Player_Item1;
        public InputAction @Item2 => m_Wrapper.m_Player_Item2;
        public InputAction @Item3 => m_Wrapper.m_Player_Item3;
        public InputAction @Item4 => m_Wrapper.m_Player_Item4;
        public InputAction @Item5 => m_Wrapper.m_Player_Item5;
        public InputAction @Item6 => m_Wrapper.m_Player_Item6;
        public InputAction @Test => m_Wrapper.m_Player_Test;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                LClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLClick;
                LClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLClick;
                LClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLClick;
                RClick.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRClick;
                RClick.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRClick;
                RClick.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRClick;
                Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                QueueHold.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQueueHold;
                QueueHold.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQueueHold;
                QueueHold.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnQueueHold;
                ZoomHold.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomHold;
                ZoomHold.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomHold;
                ZoomHold.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnZoomHold;
                Stop.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStop;
                Stop.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStop;
                Stop.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnStop;
                Ability1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility1;
                Ability1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility1;
                Ability1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility1;
                Ability2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility2;
                Ability2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility2;
                Ability2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility2;
                Ability3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility3;
                Ability3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility3;
                Ability3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility3;
                Ability4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility4;
                Ability4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility4;
                Ability4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility4;
                Ability5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility5;
                Ability5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility5;
                Ability5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility5;
                Ability6.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility6;
                Ability6.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility6;
                Ability6.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAbility6;
                Item1.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem1;
                Item1.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem1;
                Item1.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem1;
                Item2.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem2;
                Item2.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem2;
                Item2.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem2;
                Item3.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem3;
                Item3.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem3;
                Item3.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem3;
                Item4.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem4;
                Item4.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem4;
                Item4.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem4;
                Item5.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem5;
                Item5.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem5;
                Item5.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem5;
                Item6.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem6;
                Item6.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem6;
                Item6.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnItem6;
                Test.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTest;
                Test.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTest;
                Test.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnTest;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                LClick.started += instance.OnLClick;
                LClick.performed += instance.OnLClick;
                LClick.canceled += instance.OnLClick;
                RClick.started += instance.OnRClick;
                RClick.performed += instance.OnRClick;
                RClick.canceled += instance.OnRClick;
                Attack.started += instance.OnAttack;
                Attack.performed += instance.OnAttack;
                Attack.canceled += instance.OnAttack;
                QueueHold.started += instance.OnQueueHold;
                QueueHold.performed += instance.OnQueueHold;
                QueueHold.canceled += instance.OnQueueHold;
                ZoomHold.started += instance.OnZoomHold;
                ZoomHold.performed += instance.OnZoomHold;
                ZoomHold.canceled += instance.OnZoomHold;
                Stop.started += instance.OnStop;
                Stop.performed += instance.OnStop;
                Stop.canceled += instance.OnStop;
                Ability1.started += instance.OnAbility1;
                Ability1.performed += instance.OnAbility1;
                Ability1.canceled += instance.OnAbility1;
                Ability2.started += instance.OnAbility2;
                Ability2.performed += instance.OnAbility2;
                Ability2.canceled += instance.OnAbility2;
                Ability3.started += instance.OnAbility3;
                Ability3.performed += instance.OnAbility3;
                Ability3.canceled += instance.OnAbility3;
                Ability4.started += instance.OnAbility4;
                Ability4.performed += instance.OnAbility4;
                Ability4.canceled += instance.OnAbility4;
                Ability5.started += instance.OnAbility5;
                Ability5.performed += instance.OnAbility5;
                Ability5.canceled += instance.OnAbility5;
                Ability6.started += instance.OnAbility6;
                Ability6.performed += instance.OnAbility6;
                Ability6.canceled += instance.OnAbility6;
                Item1.started += instance.OnItem1;
                Item1.performed += instance.OnItem1;
                Item1.canceled += instance.OnItem1;
                Item2.started += instance.OnItem2;
                Item2.performed += instance.OnItem2;
                Item2.canceled += instance.OnItem2;
                Item3.started += instance.OnItem3;
                Item3.performed += instance.OnItem3;
                Item3.canceled += instance.OnItem3;
                Item4.started += instance.OnItem4;
                Item4.performed += instance.OnItem4;
                Item4.canceled += instance.OnItem4;
                Item5.started += instance.OnItem5;
                Item5.performed += instance.OnItem5;
                Item5.canceled += instance.OnItem5;
                Item6.started += instance.OnItem6;
                Item6.performed += instance.OnItem6;
                Item6.canceled += instance.OnItem6;
                Test.started += instance.OnTest;
                Test.performed += instance.OnTest;
                Test.canceled += instance.OnTest;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.GetControlSchemeIndex("Keyboard&Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_TouchSchemeIndex = -1;
    public InputControlScheme TouchScheme
    {
        get
        {
            if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.GetControlSchemeIndex("Touch");
            return asset.controlSchemes[m_TouchSchemeIndex];
        }
    }
    private int m_JoystickSchemeIndex = -1;
    public InputControlScheme JoystickScheme
    {
        get
        {
            if (m_JoystickSchemeIndex == -1) m_JoystickSchemeIndex = asset.GetControlSchemeIndex("Joystick");
            return asset.controlSchemes[m_JoystickSchemeIndex];
        }
    }
    public interface IUIActions
    {
        void OnNavigate(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnPoint(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
        void OnScrollWheel(InputAction.CallbackContext context);
        void OnMiddleClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnTrackedDevicePosition(InputAction.CallbackContext context);
        void OnTrackedDeviceOrientation(InputAction.CallbackContext context);
        void OnTrackedDeviceSelect(InputAction.CallbackContext context);
    }
    public interface IPlayerActions
    {
        void OnLClick(InputAction.CallbackContext context);
        void OnRClick(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnQueueHold(InputAction.CallbackContext context);
        void OnZoomHold(InputAction.CallbackContext context);
        void OnStop(InputAction.CallbackContext context);
        void OnAbility1(InputAction.CallbackContext context);
        void OnAbility2(InputAction.CallbackContext context);
        void OnAbility3(InputAction.CallbackContext context);
        void OnAbility4(InputAction.CallbackContext context);
        void OnAbility5(InputAction.CallbackContext context);
        void OnAbility6(InputAction.CallbackContext context);
        void OnItem1(InputAction.CallbackContext context);
        void OnItem2(InputAction.CallbackContext context);
        void OnItem3(InputAction.CallbackContext context);
        void OnItem4(InputAction.CallbackContext context);
        void OnItem5(InputAction.CallbackContext context);
        void OnItem6(InputAction.CallbackContext context);
        void OnTest(InputAction.CallbackContext context);
    }
}
