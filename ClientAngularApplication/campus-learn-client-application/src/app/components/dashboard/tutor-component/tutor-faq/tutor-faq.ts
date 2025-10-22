import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { TutorService } from '../../../../services/Tutor Related Services/tutor.service';
import { CreateFAQRequest, UpdateFAQRequest } from '../../../../models/Tutor Related Models/tutor-request.dtos';
import { FAQResponse } from '../../../../models/Tutor Related Models/tutor-response.dtos';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-tutor-faq',
  imports: [RouterLink, CommonModule,FormsModule],
  templateUrl: './tutor-faq.html',
  styleUrl: './tutor-faq.css'
})
export class TutorFaq {
  private sessionUser = inject(AuthService);
  private tutorService = inject(TutorService);
  
  public currentLoggedInUser?: AuthSessionUser;
  public tutorId: number = 0;
  public qualifiedModules: any[] = [];
  public faqs: FAQResponse[] = [];
  
  // UI State
  public isLoading = false;
  public showCreateForm = false;
  public showEditForm = false;
  public selectedFaq: FAQResponse | null = null;
  
  // Form Models
  public newFaq: CreateFAQRequest = {
    question: '',
    answer: '',
    moduleCode: '',
    isPublished: true
  };

  public editFaq: UpdateFAQRequest = {
    question: '',
    answer: '',
    moduleCode: '',
    isPublished: true
  };

  ngOnInit(): void {
    const user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.currentLoggedInUser = user;
      this.loadTutorData(user.userProfileID);
    }
  }

  loadTutorData(userId: number) {
    this.tutorService.getTutorIdByUserId(userId).subscribe({
      next: (res) => {
        this.tutorId = res.tutorID;
        this.loadTutorFAQs(this.tutorId);
        this.loadQualifiedModules(userId);
      },
      error: (err) => {
        console.log("Error getting tutor ID: ", err);
      }
    });
  }

  loadTutorFAQs(tutorId: number) {
    this.isLoading = true;
    this.tutorService.getAllTutorFAQs(tutorId).subscribe({
      next: (res) => {
        this.faqs = res;
        console.log('FAQ objects:', this.faqs); // Add this line to see the actual objects
        console.log('First FAQ object keys:', Object.keys(this.faqs[0])); // Add this to see property names
        this.isLoading = false;
      },
      error: (err) => {
        console.log("Error loading FAQs: ", err);
        this.isLoading = false;
      }
    });
  }

  loadQualifiedModules(userId: number) {
    this.tutorService.getQualifiedModules(userId).subscribe({
      next: (res) => {
        this.qualifiedModules = res.qualifiedModules || [];
      },
      error: (err) => {
        console.log("Error loading qualified modules: ", err);
        this.qualifiedModules = [];
      }
    });
  }

  createFAQ() {
    if (!this.newFaq.question || !this.newFaq.answer || !this.newFaq.moduleCode) return;

    this.tutorService.createFAQ(this.tutorId, this.newFaq).subscribe({
      next: (res) => {
        console.log("FAQ created successfully: ", res);
        this.showCreateForm = false;
        this.resetNewFaqForm();
        this.loadTutorFAQs(this.tutorId);
      },
      error: (err) => {
        console.log("Error creating FAQ: ", err);
      }
    });
  }

  editFAQ(faq: FAQResponse) {
  this.selectedFaq = faq;
  this.editFaq = {
    question: faq.frequentlyAskedQuestion,
    answer: faq.answer,
    moduleCode: faq.moduleCode,
    isPublished: faq.isPublished
  };
  this.showEditForm = true;
  console.log('Editing FAQ with ID:', faq.faqid); // Use faqid
}

updateFAQ() {
  if (!this.selectedFaq) return;

  this.tutorService.updateFAQ(this.selectedFaq.faqid, this.tutorId, this.editFaq).subscribe({ // Use faqid
    next: (res) => {
      console.log("FAQ updated successfully: ", res);
      this.showEditForm = false;
      this.selectedFaq = null;
      this.loadTutorFAQs(this.tutorId);
    },
    error: (err) => {
      console.log("Error updating FAQ: ", err);
    }
  });
}

deleteFAQ(faqId: number) {
  console.log('Deleting FAQ with ID:', faqId);
  console.log('Tutor ID:', this.tutorId);
  
  if (!faqId) {
    console.error('FAQ ID is undefined or null');
    return;
  }

  if (confirm('Are you sure you want to delete this FAQ?')) {
    this.tutorService.deleteFAQ(faqId, this.tutorId).subscribe({
      next: () => {
        console.log("FAQ deleted successfully");
        this.loadTutorFAQs(this.tutorId);
      },
      error: (err) => {
        console.log("Error deleting FAQ: ", err);
      }
    });
  }
}

togglePublishFAQ(faq: FAQResponse) {
  const updateRequest: UpdateFAQRequest = {
    question: faq.frequentlyAskedQuestion,
    answer: faq.answer,
    moduleCode: faq.moduleCode,
    isPublished: !faq.isPublished
  };

  this.tutorService.updateFAQ(faq.faqid, this.tutorId, updateRequest).subscribe({ // Use faqid
    next: (res) => {
      console.log("FAQ publish status updated");
      this.loadTutorFAQs(this.tutorId);
    },
    error: (err) => {
      console.log("Error updating FAQ publish status: ", err);
    }
  });
}

  resetNewFaqForm() {
    this.newFaq = {
      question: '',
      answer: '',
      moduleCode: '',
      isPublished: true
    };
  }

  cancelEdit() {
    this.showEditForm = false;
    this.selectedFaq = null;
  }

  formatDate(date: Date | string): string {
    if (!date) return 'Unknown';
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  getPublishedCount(): number {
    return this.faqs.filter(faq => faq.isPublished).length;
  }

  getDraftCount(): number {
    return this.faqs.filter(faq => !faq.isPublished).length;
  }

  onLogout() {
    this.sessionUser.onLogout();
  }
}
